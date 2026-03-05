using Asp.Versioning.ApiExplorer;
using App.DAL.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WebApp.Helpers;

namespace WebApp.Setup;

public static class MiddlewareExtensions
{
    private static readonly HashSet<string> LegacyOperationalControllers =
    [
        "Boxes", "BoxPrices", "Companies", "CompanySettings", "CompanyUsers", "CustomerExclusions", "CustomerPreferences",
        "Deliveries", "DeliveryAttempts", "DeliveryWindows", "DeliveryZones", "DietaryCategories", "MealPlans",
        "MealSelections", "MealSubscriptions", "PlatformSubscriptions", "PlatformSubscriptionTiers", "QualityComplaints",
        "Recipes", "SupportAccess", "SupportImpersonation", "SupportTickets", "SystemSettings", "WeeklyMenus", "Analytics"
    ];

    public static WebApplication UseAppMiddleware(this WebApplication app)
    {
        app.UseForwardedHeaders();

        app.UseRequestLocalization(
            options: app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value!);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.Use(async (context, next) =>
        {
            var path = context.Request.Path.Value ?? string.Empty;
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length >= 2)
            {
                var slugCandidate = segments[0];
                var controllerCandidate = segments[1];

                if (!LegacyOperationalControllers.Contains(slugCandidate)
                    && LegacyOperationalControllers.Contains(controllerCandidate)
                    && !string.Equals(slugCandidate, "system", StringComparison.OrdinalIgnoreCase))
                {
                    await using var scope = app.Services.CreateAsyncScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var slugExists = await dbContext.Companies.AnyAsync(c => c.Slug == slugCandidate);

                    if (!slugExists)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return;
                    }

                    if (context.User.Identity?.IsAuthenticated == true
                        && !context.User.HasAnyRole("SystemAdmin", "SystemBilling", "SystemSupport"))
                    {
                        var currentSlug = context.User.GetCompanySlug();
                        if (!string.IsNullOrWhiteSpace(currentSlug)
                            && !string.Equals(currentSlug, slugCandidate, StringComparison.OrdinalIgnoreCase))
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            return;
                        }
                    }
                }
            }

            if (context.User.Identity?.IsAuthenticated == true && segments.Length >= 1)
            {
                var firstSegment = segments[0];
                var isLegacyOperationalPath = LegacyOperationalControllers.Contains(firstSegment);
                if (isLegacyOperationalPath)
                {
                    var defaultSlug = context.User.GetDefaultOperationalSlug();
                    if (!string.IsNullOrWhiteSpace(defaultSlug))
                    {
                        var query = context.Request.QueryString.HasValue
                            ? context.Request.QueryString.Value
                            : string.Empty;

                        context.Response.Redirect($"/{defaultSlug}{path}{query}", permanent: false);
                        return;
                    }
                }
            }

            await next();
        });

        app.UseAuthorization();

        return app;
    }

    public static WebApplication UseAppSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant()
                );
            }
            // serve from root
            // options.RoutePrefix = string.Empty;
        });

        return app;
    }

    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        Console.WriteLine("[RouteDiagnostics] Registering slug routes: '{slug:regex(^[a-z0-9-]+$)}' and '{slug:regex(^[a-z0-9-]+$)}/{controller}/{action=Index}/{id?}'");

        app.MapStaticAssets();

        app.MapControllerRoute(
                name: "slug-entry",
                pattern: "{slug:regex(^[a-z0-9-]+$)}",
                defaults: new { controller = "Landing", action = "Entry" })
            .WithStaticAssets();

        app.MapControllerRoute(
                name: "slug-default",
                pattern: "{slug:regex(^[a-z0-9-]+$)}/{controller}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapControllerRoute(
                name: "area",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapRazorPages()
            .WithStaticAssets();

        return app;
    }
}
