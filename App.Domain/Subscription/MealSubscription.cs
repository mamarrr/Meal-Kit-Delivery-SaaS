using Base.Contracts.Domain;

namespace App.Domain.Subscription;

public class MealSubscription : BaseEntity
{
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool AutoSelectEnabled { get; set; }
    public int? NoRepeatWeeksOverride { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid CustomerId { get; set; }
    public Guid BoxId { get; set; }
    
    // Navigation Properties
    public Customer? Customer { get; set; }
    public Box? Box { get; set; }
    public ICollection<MealSelection>? MealSelections { get; set; }
    public ICollection<App.Domain.Delivery.Delivery>? Deliveries { get; set; }
}
