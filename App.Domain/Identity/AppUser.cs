using Base.Contracts.Domain;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.Identity;

public class AppUser : IdentityUser<Guid>, IBaseEntity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public bool IsDeleted { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Navigation Properties
    public ICollection<AppRefreshToken>? RefreshTokens { get; set; }
    public ICollection<CompanyAppUser>? CompanyAppUsers { get; set; }
    public ICollection<CustomerAppUser>? CustomerAppUsers { get; set; }
    public ICollection<Company>? CompaniesCreated { get; set; }
    public ICollection<CompanySettings>? CompanySettingsUpdated { get; set; }
    public ICollection<DeliveryZone>? DeliveryZonesCreated { get; set; }
    public ICollection<Recipe>? RecipesCreated { get; set; }
    public ICollection<Ingredient>? IngredientsCreated { get; set; }
    public ICollection<WeeklyMenu>? WeeklyMenusCreated { get; set; }
    public ICollection<Box>? BoxesCreated { get; set; }
    public ICollection<PlatformSubscription>? PlatformSubscriptionsCreated { get; set; }
    public ICollection<PlatformSubscriptionTier>? PlatformSubscriptionTiersCreated { get; set; }
    public ICollection<DietaryCategory>? DietaryCategoriesCreated { get; set; }
    public ICollection<RecipeIngredient>? RecipeIngredientsCreated { get; set; }
    public ICollection<RecipeDietaryCategory>? RecipeDietaryCategoriesCreated { get; set; }
    public ICollection<WeeklyMenuRecipe>? WeeklyMenuRecipesCreated { get; set; }
    public ICollection<DeliveryWindow>? DeliveryWindowsCreated { get; set; }
    public ICollection<BoxPrice>? BoxPricesCreated { get; set; }
    public ICollection<App.Domain.Delivery.Delivery>? DeliveriesCreated { get; set; }
    public ICollection<DeliveryAttempt>? DeliveryAttemptsCreated { get; set; }
}