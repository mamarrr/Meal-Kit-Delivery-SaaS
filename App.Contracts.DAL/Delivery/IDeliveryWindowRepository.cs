using App.Domain.Delivery;
using Contracts.DAL;

namespace App.Contracts.DAL.Delivery;

/// <summary>
/// Repository interface for DeliveryWindow aggregate.
/// </summary>
public interface IDeliveryWindowRepository : IRepository<DeliveryWindow>
{
    /// <summary>
    /// Gets all delivery windows for a specific delivery zone.
    /// </summary>
    /// <param name="deliveryZoneId">The delivery zone ID.</param>
    /// <returns>A collection of delivery windows for the zone.</returns>
    Task<ICollection<DeliveryWindow>> GetAllByDeliveryZoneIdAsync(Guid deliveryZoneId);

    /// <summary>
    /// Gets all active delivery windows for a specific delivery zone.
    /// </summary>
    /// <param name="deliveryZoneId">The delivery zone ID.</param>
    /// <returns>A collection of active delivery windows for the zone.</returns>
    Task<ICollection<DeliveryWindow>> GetActiveByDeliveryZoneIdAsync(Guid deliveryZoneId);

    /// <summary>
    /// Gets all delivery windows for a company via its zones.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of delivery windows belonging to the company's zones.</returns>
    Task<ICollection<DeliveryWindow>> GetAllByCompanyIdAsync(Guid companyId);
}
