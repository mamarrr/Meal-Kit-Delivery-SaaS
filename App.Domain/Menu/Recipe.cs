using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Menu;

public class Recipe : BaseEntity, ITenantProvider
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int DefaultServings { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid CompanyId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public Company? Company { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
    public NutritionalInfo? NutritionalInfo { get; set; }
    public ICollection<RecipeIngredient>? RecipeIngredients { get; set; }
    public ICollection<RecipeDietaryCategory>? RecipeDietaryCategories { get; set; }
    public ICollection<WeeklyMenuRecipe>? WeeklyMenuRecipes { get; set; }
    public ICollection<MealSelection>? MealSelections { get; set; }
    public ICollection<Rating>? Ratings { get; set; }
}
