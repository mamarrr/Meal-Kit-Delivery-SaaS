using App.Contracts.BLL.Subscription;

namespace WebApp.ViewModels.Subscription;

public class PricingProductsIndexViewModel
{
    public Guid CompanyId { get; set; }
    public string CompanySlug { get; set; } = string.Empty;

    public List<PricingBoxListItemDto> Boxes { get; set; } = [];
    public List<PricingConfigurationDto> PricingConfigurations { get; set; } = [];
    public List<PricingDietaryCategoryOptionDto> DietaryCategories { get; set; } = [];
    public List<PricingAdjustmentListItemDto> DeliveryFees { get; set; } = [];
    public List<PricingAdjustmentListItemDto> Discounts { get; set; } = [];

    public PricingBoxUpsertInput BoxForm { get; set; } = new();
    public PricingBoxPriceUpsertInput PriceForm { get; set; } = new();
    public PricingAdjustmentUpsertInput AdjustmentForm { get; set; } = new();
}

public class PricingBoxUpsertInput
{
    public Guid? BoxId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MealsCount { get; set; } = 3;
    public int PeopleCount { get; set; } = 2;
    public List<Guid> AllowedDietaryCategoryIds { get; set; } = [];
    public bool IsActive { get; set; } = true;
}

public class PricingBoxPriceUpsertInput
{
    public Guid? BoxPriceId { get; set; }
    public Guid BoxId { get; set; }
    public string PricingName { get; set; } = string.Empty;
    public decimal PriceAmount { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PricingAdjustmentUpsertInput
{
    public Guid? AdjustmentId { get; set; }
    public string AdjustmentType { get; set; } = PricingConstants.DeliveryFeeType;
    public string Label { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPercentage { get; set; }
    public bool IsActive { get; set; } = true;
}

