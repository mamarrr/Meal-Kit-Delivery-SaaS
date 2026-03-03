using Base.Contracts.Domain;

namespace App.Domain.Menu;

public class MealSelection : BaseEntity
{
    public bool SelectedAutomatically { get; set; }
    public DateTime SelectedAt { get; set; }
    public DateTime? LockedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid MealSubscriptionId { get; set; }
    public Guid WeeklyMenuId { get; set; }
    public Guid RecipeId { get; set; }
    
    // Navigation Properties
    public MealSubscription? MealSubscription { get; set; }
    public WeeklyMenu? WeeklyMenu { get; set; }
    public Recipe? Recipe { get; set; }
    public ICollection<App.Domain.Delivery.Delivery>? Deliveries { get; set; }
}
