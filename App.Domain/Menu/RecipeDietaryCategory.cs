using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Menu;

public class RecipeDietaryCategory : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid DietaryCategoryId { get; set; }
    public Guid RecipeId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public DietaryCategory? DietaryCategory { get; set; }
    public Recipe? Recipe { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
}
