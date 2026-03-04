namespace WebApp.ViewModels.MealPlans;

public class MealPlanDiscoveryViewModel
{
    public Guid CompanyId { get; set; }

    public IReadOnlyList<MealPlanListItemViewModel> Plans { get; set; } = [];
}
