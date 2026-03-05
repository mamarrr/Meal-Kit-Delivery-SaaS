using App.Domain.Menu;
using Contracts.DAL;

namespace App.Contracts.DAL.Menu;

/// <summary>
/// Repository interface for Ingredient aggregate.
/// </summary>
public interface IIngredientRepository : IRepository<Ingredient>
{
    /// <summary>
    /// Gets all ingredients for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of ingredients belonging to the company.</returns>
    Task<ICollection<Ingredient>> GetAllByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Gets all ingredients for a collection of companies.
    /// </summary>
    /// <param name="companyIds">The company IDs.</param>
    /// <returns>A collection of ingredients belonging to the companies.</returns>
    Task<ICollection<Ingredient>> GetAllByCompanyIdsAsync(IEnumerable<Guid> companyIds);

    Task<Ingredient?> GetByNameAsync(Guid companyId, string name);
    Task<ICollection<Ingredient>> GetByIdsAsync(Guid companyId, ICollection<Guid> ids);
}
