namespace App.Contracts.BLL.Subscription;

public interface IPricingProductsService
{
    Task<PricingProductsPageDto> GetPageAsync(Guid companyId);

    Task UpsertBoxAsync(Guid companyId, Guid actorId, PricingBoxUpsertDto dto);
    Task UpsertPriceAsync(Guid companyId, Guid actorId, PricingBoxPriceUpsertDto dto);
    Task UpsertAdjustmentAsync(Guid companyId, Guid actorId, PricingAdjustmentUpsertDto dto);

    Task DeactivateBoxAsync(Guid companyId, Guid boxId);
    Task DeactivateAdjustmentAsync(Guid companyId, Guid adjustmentId);
}

public sealed class PricingProductsPageDto
{
    public IReadOnlyCollection<PricingBoxListItemDto> Boxes { get; init; } = [];
    public IReadOnlyCollection<PricingConfigurationDto> PricingConfigurations { get; init; } = [];
    public IReadOnlyCollection<PricingDietaryCategoryOptionDto> DietaryCategories { get; init; } = [];
    public IReadOnlyCollection<PricingAdjustmentListItemDto> DeliveryFees { get; init; } = [];
    public IReadOnlyCollection<PricingAdjustmentListItemDto> Discounts { get; init; } = [];
}

public sealed class PricingBoxListItemDto
{
    public Guid BoxId { get; init; }
    public int MealsCount { get; init; }
    public int PeopleCount { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public IReadOnlyCollection<Guid> AllowedDietaryCategoryIds { get; init; } = [];
}

public sealed class PricingConfigurationDto
{
    public string PricingName { get; init; } = string.Empty;
    public IReadOnlyCollection<PricingBoxPriceRowDto> BoxPrices { get; init; } = [];
}

public sealed class PricingBoxPriceRowDto
{
    public Guid? BoxPriceId { get; init; }
    public Guid BoxId { get; init; }
    public string BoxLabel { get; init; } = string.Empty;
    public decimal? PriceAmount { get; init; }
    public bool IsActive { get; init; }
}

public sealed class PricingDietaryCategoryOptionDto
{
    public Guid DietaryCategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
}

public sealed class PricingAdjustmentListItemDto
{
    public Guid AdjustmentId { get; init; }
    public string AdjustmentType { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public bool IsPercentage { get; init; }
    public bool IsActive { get; init; }
}

public sealed class PricingBoxUpsertDto
{
    public Guid? BoxId { get; init; }
    public string Name { get; init; } = string.Empty;
    public int MealsCount { get; init; }
    public int PeopleCount { get; init; }
    public IReadOnlyCollection<Guid> AllowedDietaryCategoryIds { get; init; } = [];
    public bool IsActive { get; init; } = true;
}

public sealed class PricingBoxPriceUpsertDto
{
    public Guid? BoxPriceId { get; init; }
    public Guid BoxId { get; init; }
    public string PricingName { get; init; } = string.Empty;
    public decimal PriceAmount { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class PricingAdjustmentUpsertDto
{
    public Guid? AdjustmentId { get; init; }
    public string AdjustmentType { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public bool IsPercentage { get; init; }
    public bool IsActive { get; init; } = true;
}

