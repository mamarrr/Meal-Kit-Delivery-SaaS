using App.BLL.Core;
using App.BLL.Delivery;
using App.BLL.Identity;
using App.BLL.Menu;
using App.BLL.Subscription;
using App.BLL.Support;
using App.Contracts.BLL.Core;
using App.Contracts.BLL.Delivery;
using App.Contracts.BLL.Identity;
using App.Contracts.BLL.Menu;
using App.Contracts.BLL.Subscription;
using App.Contracts.BLL.Support;
using Microsoft.Extensions.DependencyInjection;

namespace App.BLL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBllServices(this IServiceCollection services)
    {
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICompanyAppUserService, CompanyAppUserService>();
        services.AddScoped<ICompanySettingsService, CompanySettingsService>();
        services.AddScoped<ICustomerAppUserService, CustomerAppUserService>();

        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IIngredientService, IngredientService>();
        services.AddScoped<IDietaryCategoryService, DietaryCategoryService>();
        services.AddScoped<ICustomerPreferenceService, CustomerPreferenceService>();
        services.AddScoped<ICustomerExclusionService, CustomerExclusionService>();
        services.AddScoped<IWeeklyMenuService, WeeklyMenuService>();
        services.AddScoped<IMealSelectionService, MealSelectionService>();

        services.AddScoped<IDeliveryService, DeliveryService>();
        services.AddScoped<IDeliveryZoneService, DeliveryZoneService>();
        services.AddScoped<IQualityComplaintService, QualityComplaintService>();

        services.AddScoped<IBoxService, BoxService>();
        services.AddScoped<IMealSubscriptionService, MealSubscriptionService>();
        services.AddScoped<IPlatformSubscriptionService, PlatformSubscriptionService>();
        services.AddScoped<IPlatformSubscriptionTierService, PlatformSubscriptionTierService>();

        services.AddScoped<IAppUserService, AppUserService>();
        services.AddScoped<IAppRefreshTokenService, AppRefreshTokenService>();

        services.AddScoped<ISystemSettingService, SystemSettingService>();
        services.AddScoped<ITenantSupportAccessService, TenantSupportAccessService>();
        services.AddScoped<ISupportTicketStatusService, SupportTicketStatusService>();
        services.AddScoped<ISupportTicketService, SupportTicketService>();
        services.AddScoped<ISupportImpersonationSessionService, SupportImpersonationSessionService>();
        services.AddScoped<ISystemAnalyticsSnapshotService, SystemAnalyticsSnapshotService>();

        return services;
    }
}

