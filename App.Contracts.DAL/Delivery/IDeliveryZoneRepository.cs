using App.Domain.Delivery;
using Contracts.DAL;

namespace App.Contracts.DAL.Delivery;

/// <summary>
/// Repository interface for DeliveryZone aggregate.
/// </summary>
public interface IDeliveryZoneRepository : IRepository<DeliveryZone>
{
    /// <summary>
    /// Gets all delivery zones for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of delivery zones belonging to the company.</returns>
    Task<ICollection<DeliveryZone>> GetAllByCompanyIdAsync(Guid companyId);
}
