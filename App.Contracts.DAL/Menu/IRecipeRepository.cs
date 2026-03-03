using App.Domain.Menu;
using Contracts.DAL;

namespace App.Contracts.DAL.Menu;

/// <summary>
/// Repository interface for Recipe aggregate.
/// </summary>
public interface IRecipeRepository : IRepository<Recipe>
{
    /// <summary>
    /// Gets all recipes for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of recipes belonging to the company.</returns>
    Task<ICollection<Recipe>> GetAllByCompanyIdAsync(Guid companyId);
}
