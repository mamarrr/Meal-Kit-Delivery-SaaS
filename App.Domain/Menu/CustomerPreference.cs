using Base.Contracts.Domain;

namespace App.Domain.Menu;

public class CustomerPreference : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid DietaryCategoryId { get; set; }
    public Guid CustomerId { get; set; }
    
    // Navigation Properties
    public DietaryCategory? DietaryCategory { get; set; }
    public Customer? Customer { get; set; }
}
