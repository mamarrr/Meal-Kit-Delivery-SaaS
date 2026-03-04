using App.Contracts.DAL.Core;
using App.Contracts.DAL.Delivery;
using App.Contracts.DAL.Identity;
using App.Contracts.DAL.Menu;
using App.Contracts.DAL.Subscription;
using App.Contracts.DAL.Support;
using App.DAL.EF.Repositories.Core;
using App.DAL.EF.Repositories.Delivery;
using App.DAL.EF.Repositories.Identity;
using App.DAL.EF.Repositories.Menu;
using App.DAL.EF.Repositories.Subscription;
using App.DAL.EF.Repositories.Support;
using Microsoft.Extensions.DependencyInjection;

namespace App.DAL.EF;

/// <summary>
/// Extension methods for registering DAL services in the dependency injection container.
/// </summary>
public static class DalServiceExtensions
{
    /// <summary>
    /// Adds all DAL repository services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDalRepositories(this IServiceCollection services)
    {
        // Core domain repositories
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanySettingsRepository, CompanySettingsRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICompanyAppUserRepository, CompanyAppUserRepository>();
        services.AddScoped<ICompanyRoleRepository, CompanyRoleRepository>();
        services.AddScoped<ICustomerAppUserRepository, CustomerAppUserRepository>();
        
        // Menu domain repositories
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IDietaryCategoryRepository, DietaryCategoryRepository>();
        services.AddScoped<ICustomerPreferenceRepository, CustomerPreferenceRepository>();
        services.AddScoped<ICustomerExclusionRepository, CustomerExclusionRepository>();
        services.AddScoped<IWeeklyMenuRepository, WeeklyMenuRepository>();
        services.AddScoped<IMealSelectionRepository, MealSelectionRepository>();
        
        // Subscription domain repositories
        services.AddScoped<IBoxRepository, BoxRepository>();
        services.AddScoped<IBoxPriceRepository, BoxPriceRepository>();
        services.AddScoped<IMealSubscriptionRepository, MealSubscriptionRepository>();
        services.AddScoped<IPlatformSubscriptionRepository, PlatformSubscriptionRepository>();
        services.AddScoped<IPlatformSubscriptionStatusRepository, PlatformSubscriptionStatusRepository>();
        services.AddScoped<IPlatformSubscriptionTierRepository, PlatformSubscriptionTierRepository>();
        
        // Delivery domain repositories
        services.AddScoped<IDeliveryZoneRepository, DeliveryZoneRepository>();
        services.AddScoped<IDeliveryWindowRepository, DeliveryWindowRepository>();
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddScoped<IQualityComplaintRepository, QualityComplaintRepository>();
        
        // Identity domain repositories
        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IAppRefreshTokenRepository, AppRefreshTokenRepository>();

        // Support/system repositories
        services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
        services.AddScoped<ITenantSupportAccessRepository, TenantSupportAccessRepository>();
        services.AddScoped<ISupportTicketStatusRepository, SupportTicketStatusRepository>();
        services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
        services.AddScoped<ISupportImpersonationSessionRepository, SupportImpersonationSessionRepository>();
        services.AddScoped<ISystemAnalyticsSnapshotRepository, SystemAnalyticsSnapshotRepository>();

        return services;
    }
}
