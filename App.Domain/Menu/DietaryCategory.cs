using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Menu;

public class DietaryCategory : BaseEntity, ITenantProvider
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
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
    public ICollection<RecipeDietaryCategory>? RecipeDietaryCategories { get; set; }
    public ICollection<CustomerPreference>? CustomerPreferences { get; set; }
}
