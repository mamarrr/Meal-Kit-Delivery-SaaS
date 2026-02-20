using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using App.DAL.EF;
using App.DAL.EF.Seeding;
using App.Domain;
using App.Domain.Identity;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using NuGet.Packaging;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApp;
using WebApp.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// used for older style [Column(TypeName = "jsonb")] for LangStr
#pragma warning disable CS0618 // Type or member is obsolete
NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
#pragma warning restore CS0618 // Type or member is obsolete

if (builder.Environment.IsProduction())
{
    builder.Services
        .AddDbContext<AppDbContext>(options => options
            .UseNpgsql(
                connectionString,
                o => { o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }
            )
            .ConfigureWarnings(w =>
                w.Throw(RelationalEventId.MultipleCollectionIncludeWarning)
            )
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution)
        );
}
else
{
    builder.Services
        .AddDbContext<AppDbContext>(options => options
            .UseNpgsql(
                connectionString,
                o => { o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }
            )
            .ConfigureWarnings(w =>
                w.Throw(RelationalEventId.MultipleCollectionIncludeWarning)
            )
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution)
        );
}


builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDataProtection().PersistKeysToDbContext<AppDbContext>();

builder.Services.AddSingleton<AppNameService>();


builder.Services
    .AddIdentity<AppUser, AppRole>(options => options.SignIn.RequireConfirmedAccount = false)
    //.AddErrorDescriber<LocalizedIdentityErrorDescriber>()
    .AddDefaultUI()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpClient();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardLimit = null; // null = no limit (0 means don't process any!)
        options.ForwardedHeaders = ForwardedHeaders.All;
        // Trust all IP addresses - safe when behind a known reverse proxy
        options.KnownProxies.Clear();
        options.KnownIPNetworks.Clear();
        options.KnownIPNetworks.AddRange(
            new List<System.Net.IPNetwork>()
            {
                new System.Net.IPNetwork(IPAddress.Any, 0),
                new System.Net.IPNetwork(IPAddress.IPv6Any, 0)
            }
        );
    }
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsAllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("X-Version", "X-Version-Created-At");
    });
});


var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    // in case of no explicit version
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

apiVersioningBuilder.AddApiExplorer(options =>
{
    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
    // note: the specified format code will format the version as "'v'major[.minor][-status]"
    options.GroupNameFormat = "'v'VVV";

    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
    // can also be used to control the format of the API version in route templates
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

var supportedCultures = builder.Configuration
    .GetSection("SupportedCultures")
    .GetChildren()
    .Select(x => new CultureInfo(x.Value!))
    .ToArray();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    // datetime and currency support
    options.SupportedCultures = supportedCultures;
    // UI translated strings
    options.SupportedUICultures = supportedCultures;
    // if nothing is found, use this
    options.DefaultRequestCulture = new RequestCulture("et-EE", "et-EE");
    options.SetDefaultCulture("et-EE");

    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        // Order is important, it's in which order they will be evaluated
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

LangStr.DefaultCulture = builder.Configuration.GetValue<string>("LangStrDefaultCulture") ?? "en";


// ======================================================================================================
var app = builder.Build();
// ======================================================================================================

SetupAppData(app, app.Environment, app.Configuration);

app.UseForwardedHeaders();

//app.UseHttpLogging();

app.UseRequestLocalization(options: app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value!);


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

app.UseAuthorization();


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

app.MapStaticAssets();

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

app.Run();

// ======================================================================================================
return;
// ======================================================================================================

static void SetupAppData(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
{
    using var serviceScope = app.ApplicationServices
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope();
    var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<IApplicationBuilder>>();

    using var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory") return;

    WaitDbConnection(context, logger);

    using var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    using var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    
    if (configuration.GetValue<bool>("DataInitialization:DropDatabase"))
    {
        logger.LogWarning("DropDatabase");
        AppDataInit.DeleteDatabase(context);
    }

    if (configuration.GetValue<bool>("DataInitialization:MigrateDatabase"))
    {
        logger.LogInformation("MigrateDatabase");
        AppDataInit.MigrateDatabase(context);
    }

    if (configuration.GetValue<bool>("DataInitialization:SeedIdentity"))
    {
        logger.LogInformation("SeedIdentity");
        AppDataInit.SeedIdentity(userManager, roleManager);
    }

    if (configuration.GetValue<bool>("DataInitialization:SeedData"))
    {
        logger.LogInformation("SeedData");
        AppDataInit.SeedAppData(context);
    }
}

static void WaitDbConnection(AppDbContext ctx, ILogger<IApplicationBuilder> logger)
{
    // TODO: Login failed for user 'sa'. Reason: Failed to open the explicitly specified database 'XYZ'. [CLIENT: 172.18.0.3]
    // could actually log in, but db was not there - migrations where not applied yet

    // maybe Database.OpenConnection

    while (true)
    {
        try
        {
            ctx.Database.OpenConnection();
            ctx.Database.CloseConnection();
            return;
        }
/*
        catch (SqlException e)
        {
            // db server is not yet up
            // A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections. (provider: TCP Provider, error: 40 - Could not open a connection to SQL Server)
            // its up, but database is not there - apply migration
            // Cannot open database "XYZ" requested by the login. The login failed. Login failed for user 'sa'.

            logger.LogWarning("Checked db connection. Got: {}", e.Message);
            if (e.Message.Contains("The login failed."))
            {
                logger.LogWarning("Applying migration, probably db is not there (but server is)");
                return;
            }

            logger.LogWarning("Waiting for db connection. Sleep 1 sec");
            System.Threading.Thread.Sleep(1000);
        }
*/
        catch (Npgsql.PostgresException e)
        {
            logger.LogWarning("Checked postgres db connection. Got: {}", e.Message);

            if (e.Message.Contains("does not exist"))
            {
                logger.LogWarning("Applying migration, probably db is not there (but server is)");
                return;
            }

            logger.LogWarning("Waiting for db connection. Sleep 1 sec");
            Thread.Sleep(1000);
        }
    }
}

// this is needed for unit testing
// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program
{
}