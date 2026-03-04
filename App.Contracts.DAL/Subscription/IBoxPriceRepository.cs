using App.Domain.Subscription;
using Contracts.DAL;

namespace App.Contracts.DAL.Subscription;

/// <summary>
/// Repository interface for BoxPrice aggregate.
/// </summary>
public interface IBoxPriceRepository : IRepository<BoxPrice>
{
    /// <summary>
    /// Gets all box prices for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of box prices belonging to the company.</returns>
    Task<ICollection<BoxPrice>> GetAllByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Gets all box prices for a specific box.
    /// </summary>
    /// <param name="boxId">The box ID.</param>
    /// <returns>A collection of box prices for the box.</returns>
    Task<ICollection<BoxPrice>> GetAllByBoxIdAsync(Guid boxId);

    /// <summary>
    /// Gets all active box prices for a specific box within company scope.
    /// </summary>
    /// <param name="boxId">The box ID.</param>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of active box prices.</returns>
    Task<ICollection<BoxPrice>> GetActiveByBoxIdAsync(Guid boxId, Guid companyId);
}
