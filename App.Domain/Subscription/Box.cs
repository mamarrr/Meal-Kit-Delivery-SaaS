using App.Domain.Identity;
using App.Domain.Menu;
using Base.Contracts.Domain;

namespace App.Domain.Subscription;

public class Box : BaseEntity, ITenantProvider
{
    public int MealsCount { get; set; }
    public int PeopleCount { get; set; }
    public string DisplayName { get; set; } = default!;
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid CreatedByAppUserId { get; set; }
    public Guid CompanyId { get; set; }
    
    // Navigation Properties
    public AppUser? CreatedByAppUser { get; set; }
    public Company? Company { get; set; }
    public ICollection<MealSubscription>? MealSubscriptions { get; set; }
    public ICollection<BoxPrice>? BoxPrices { get; set; }
    public ICollection<BoxDietaryCategory>? BoxDietaryCategories { get; set; }
    public ICollection<App.Domain.Delivery.Delivery>? Deliveries { get; set; }
}
