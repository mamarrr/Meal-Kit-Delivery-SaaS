using App.DAL.EF;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp.Setup;

public static class IdentitySetupExtensions
{
    public static IServiceCollection AddAppIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<AppUser, AppRole>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddDefaultUI()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // Authorization policies aligned with workflow coverage matrix roles
        services.AddAuthorization(options =>
        {
            // Customer workflows (CUST-*)
            options.AddPolicy("CustomerAccess", policy =>
                policy.RequireRole("Customer"));

            // Company workflows (COMP-*)
            options.AddPolicy("CompanyOwner", policy =>
                policy.RequireRole("CompanyOwner"));
            options.AddPolicy("CompanyAdmin", policy =>
                policy.RequireRole("CompanyOwner", "CompanyAdmin"));
            options.AddPolicy("CompanyManager", policy =>
                policy.RequireRole("CompanyOwner", "CompanyAdmin", "CompanyManager"));
            options.AddPolicy("CompanyEmployee", policy =>
                policy.RequireRole("CompanyOwner", "CompanyAdmin", "CompanyManager", "CompanyEmployee"));

            // System workflows (SYS-*)
            options.AddPolicy("SystemAdmin", policy =>
                policy.RequireRole("SystemAdmin"));
            options.AddPolicy("SystemBilling", policy =>
                policy.RequireRole("SystemAdmin", "SystemBilling"));
            options.AddPolicy("SystemSupport", policy =>
                policy.RequireRole("SystemAdmin", "SystemSupport"));
        });

        return services;
    }
}