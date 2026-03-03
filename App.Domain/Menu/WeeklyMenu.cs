using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Menu;

public class WeeklyMenu : BaseEntity, ITenantProvider
{
    public DateTime WeekStartDate { get; set; }
    public DateTime SelectionDeadlineAt { get; set; }
    public int TotalRecipes { get; set; }
    public bool IsPublished { get; set; }
    public DateTime PublishedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid CompanyId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public Company? Company { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
    public ICollection<WeeklyMenuRecipe>? WeeklyMenuRecipes { get; set; }
    public ICollection<MealSelection>? MealSelections { get; set; }
    public ICollection<App.Domain.Delivery.Delivery>? Deliveries { get; set; }
}
