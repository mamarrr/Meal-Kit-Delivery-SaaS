using Base.Contracts.Domain;

namespace App.Domain.Menu;

public class WeeklyMenuRuleConfig : BaseEntity, ITenantProvider
{
    public int RecipesPerCategory { get; set; }
    public int NoRepeatWeeks { get; set; } = 8;
    public int SelectionDeadlineDaysBeforeWeekStart { get; set; } = 2;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Foreign Keys
    public Guid CompanyId { get; set; }

    // Navigation Properties
    public Company? Company { get; set; }
}
