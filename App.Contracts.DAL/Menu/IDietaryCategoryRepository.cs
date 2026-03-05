using App.Domain.Menu;
using Contracts.DAL;

namespace App.Contracts.DAL.Menu;

/// <summary>
/// Repository interface for DietaryCategory aggregate.
/// </summary>
public interface IDietaryCategoryRepository : IRepository<DietaryCategory>
{
    /// <summary>
    /// Gets all dietary categories for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of dietary categories belonging to the company.</returns>
    Task<ICollection<DietaryCategory>> GetAllByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Gets all dietary categories for a collection of companies.
    /// </summary>
    /// <param name="companyIds">The company IDs.</param>
    /// <returns>A collection of dietary categories belonging to the companies.</returns>
    Task<ICollection<DietaryCategory>> GetAllByCompanyIdsAsync(IEnumerable<Guid> companyIds);
}
