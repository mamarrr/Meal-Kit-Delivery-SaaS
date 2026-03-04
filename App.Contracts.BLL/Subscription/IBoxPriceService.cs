using App.Domain.Subscription;

namespace App.Contracts.BLL.Subscription;

public interface IBoxPriceService : ITenantEntityService<BoxPrice>
{
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
