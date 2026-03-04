namespace WebApp.ViewModels.MealPlans;

public class MealPlanListItemViewModel
{
    public Guid BoxPriceId { get; set; }

    public Guid BoxId { get; set; }

    public string BoxDisplayName { get; set; } = string.Empty;

    public int MealsCount { get; set; }

    public int PeopleCount { get; set; }

    public decimal PriceAmount { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public string SubscribeUrl { get; set; } = "/MealSubscriptions/Create";
}
