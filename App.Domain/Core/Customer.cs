using Base.Contracts.Domain;

namespace App.Domain.Core;

public class Customer : BaseEntity, ITenantProvider
{
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public string AddressLine { get; set; } = default!;
    public string City { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string Country { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid CompanyId { get; set; }
    
    // Navigation Properties
    public Company? Company { get; set; }
    public ICollection<MealSubscription>? MealSubscriptions { get; set; }
    public ICollection<App.Domain.Delivery.Delivery>? Deliveries { get; set; }
    public ICollection<Rating>? Ratings { get; set; }
    public ICollection<CustomerExclusion>? CustomerExclusions { get; set; }
    public ICollection<CustomerPreference>? CustomerPreferences { get; set; }
    public ICollection<QualityComplaint>? QualityComplaints { get; set; }
    public ICollection<CustomerAppUser>? CustomerAppUsers { get; set; }
}
