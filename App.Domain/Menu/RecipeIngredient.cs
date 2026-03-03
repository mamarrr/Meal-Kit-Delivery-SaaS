using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Menu;

public class RecipeIngredient : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid RecipeId { get; set; }
    public Guid IngredientId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public Recipe? Recipe { get; set; }
    public Ingredient? Ingredient { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
}
