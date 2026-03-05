using App.Domain.Subscription;
using Contracts.DAL;

namespace App.Contracts.DAL.Subscription;

/// <summary>
/// Repository interface for Box aggregate.
/// </summary>
public interface IBoxRepository : IRepository<Box>
{
    /// <summary>
    /// Gets all boxes for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of boxes belonging to the company.</returns>
    Task<ICollection<Box>> GetAllByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Counts active (non-deleted) boxes for a company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>Number of active boxes in the company scope.</returns>
    Task<int> CountActiveByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Gets customer-discoverable active boxes with optional filtering.
    /// </summary>
    /// <param name="companyIds">Optional company scope filter.</param>
    /// <param name="minPrice">Optional minimum active price filter.</param>
    /// <param name="maxPrice">Optional maximum active price filter.</param>
    /// <param name="dietaryCategoryIds">Optional dietary category filter (any-match).</param>
    /// <returns>A collection of discoverable boxes with related company/price/category data.</returns>
    Task<ICollection<Box>> GetDiscoverableBoxesAsync(
        IReadOnlyCollection<Guid>? companyIds,
        decimal? minPrice,
        decimal? maxPrice,
        IReadOnlyCollection<Guid>? dietaryCategoryIds);
}
