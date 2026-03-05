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

    Task<Ingredient?> GetByNameAsync(Guid companyId, string name);
    Task<ICollection<Ingredient>> GetByIdsAsync(Guid companyId, ICollection<Guid> ids);
}
