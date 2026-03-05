using App.Contracts.BLL.Subscription;
using App.Domain.Menu;
using App.Domain.Subscription;

namespace WebApp.ViewModels.Subscription;

public sealed class DiscoverSubscriptionsViewModel
{
    public string? CompanySlug { get; set; }

    public List<CustomerDiscoverableBoxDto> Boxes { get; set; } = [];
    public List<CompanyOptionItem> CompanyOptions { get; set; } = [];

    public List<Guid> SelectedCompanyIds { get; set; } = [];
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<Guid> SelectedDietaryCategoryIds { get; set; } = [];

    public Guid SubscribeBoxId { get; set; }
}

public sealed class ManageSubscriptionsViewModel
{
    public string? CompanySlug { get; set; }
    public List<MealSubscription> Subscriptions { get; set; } = [];
    public MealSubscription? SelectedSubscription { get; set; }
    public Guid? SelectedSubscriptionId { get; set; }
}

public sealed class CompanyOptionItem
{
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
}

public sealed class CustomerPreferencesViewModel
{
    public List<DietaryCategory> AvailableDietaryCategories { get; set; } = [];
    public List<Ingredient> AvailableIngredients { get; set; } = [];

    public List<Guid> SelectedPreferenceIds { get; set; } = [];
    public List<Guid> SelectedExclusionIds { get; set; } = [];

    public decimal? MinCaloriesKcal { get; set; }
    public decimal? MaxCaloriesKcal { get; set; }
    public decimal? MinProteinG { get; set; }
    public decimal? MaxProteinG { get; set; }
    public decimal? MinCarbsG { get; set; }
    public decimal? MaxCarbsG { get; set; }
    public decimal? MinFatG { get; set; }
    public decimal? MaxFatG { get; set; }
    public decimal? MinFiberG { get; set; }
    public decimal? MaxFiberG { get; set; }
    public decimal? MinSodiumMg { get; set; }
    public decimal? MaxSodiumMg { get; set; }
}

