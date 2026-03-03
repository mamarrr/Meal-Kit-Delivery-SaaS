using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Core;

public class Company : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string RegistrationNumber { get; set; } = default!;
    public string ContactEmail { get; set; } = default!;
    public string ContactPhone { get; set; } = default!;
    public string WebSiteUrl { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? DeActivatedAt { get; set; }
    
    // Foreign Keys
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public AppUser? CreatedByAppUser { get; set; }
    public CompanySettings? Settings { get; set; }
    public ICollection<Customer>? Customers { get; set; }
    public ICollection<DeliveryZone>? DeliveryZones { get; set; }
    public ICollection<Recipe>? Recipes { get; set; }
    public ICollection<Ingredient>? Ingredients { get; set; }
    public ICollection<WeeklyMenu>? WeeklyMenus { get; set; }
    public ICollection<Box>? Boxes { get; set; }
    public ICollection<PlatformSubscription>? PlatformSubscriptions { get; set; }
    public ICollection<DietaryCategory>? DietaryCategories { get; set; }
    public ICollection<QualityComplaint>? QualityComplaints { get; set; }
    public ICollection<CompanyAppUser>? CompanyAppUsers { get; set; }
    public ICollection<BoxPrice>? BoxPrices { get; set; }
    public ICollection<App.Domain.Delivery.Delivery>? Deliveries { get; set; }
}
