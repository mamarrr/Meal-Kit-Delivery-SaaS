using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Core;

public class CustomerAppUser : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid CustomerId { get; set; }
    public Guid AppUserId { get; set; }
    
    // Navigation Properties
    public Customer? Customer { get; set; }
    public AppUser? AppUser { get; set; }
}
