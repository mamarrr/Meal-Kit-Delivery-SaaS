using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Menu;

public class WeeklyMenuRecipe : BaseEntity
{
    public Guid? DietaryCategoryId { get; set; }
    public int? DisplayOrder { get; set; }
    public bool IsFeatured { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid RecipeId { get; set; }
    public Guid WeeklyMenuId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public DietaryCategory? DietaryCategory { get; set; }
    public Recipe? Recipe { get; set; }
    public WeeklyMenu? WeeklyMenu { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
}
