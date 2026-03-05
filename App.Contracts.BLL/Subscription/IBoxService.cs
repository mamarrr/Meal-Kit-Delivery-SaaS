using App.Domain.Subscription;

namespace App.Contracts.BLL.Subscription;

public interface IBoxService : ITenantEntityService<Box>
{
    /// <summary>
    /// Counts active (non-deleted) boxes for a company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>Number of active boxes.</returns>
    Task<int> CountActiveByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Gets customer-discoverable public box offerings with optional filters.
    /// </summary>
    Task<ICollection<CustomerDiscoverableBoxDto>> GetDiscoverableBoxesAsync(CustomerBoxDiscoveryFilterDto filter);
}

public sealed class CustomerBoxDiscoveryFilterDto
{
    public IReadOnlyCollection<Guid> CompanyIds { get; init; } = [];
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public IReadOnlyCollection<Guid> DietaryCategoryIds { get; init; } = [];
}

public sealed class CustomerDiscoverableBoxDto
{
    public Guid BoxId { get; init; }
    public Guid CompanyId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public int MealsCount { get; init; }
    public int PeopleCount { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public decimal? ActivePrice { get; init; }
    public IReadOnlyCollection<Guid> DietaryCategoryIds { get; init; } = [];
}

