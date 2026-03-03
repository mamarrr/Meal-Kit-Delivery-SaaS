using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Menu;

public class Ingredient : BaseEntity, ITenantProvider
{
    public string Name { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid CompanyId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public Company? Company { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
    public ICollection<RecipeIngredient>? RecipeIngredients { get; set; }
    public ICollection<CustomerExclusion>? CustomerExclusions { get; set; }
}
