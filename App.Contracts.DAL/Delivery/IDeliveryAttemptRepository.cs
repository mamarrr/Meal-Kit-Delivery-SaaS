using Contracts.DAL;

namespace App.Contracts.DAL.Delivery;

/// <summary>
/// Repository interface for DeliveryAttempt aggregate.
/// </summary>
public interface IDeliveryAttemptRepository : IRepository<App.Domain.Delivery.DeliveryAttempt>
{
    /// <summary>
    /// Gets all delivery attempts for a specific delivery.
    /// </summary>
    /// <param name="deliveryId">The delivery ID.</param>
    /// <returns>A collection of attempts for the delivery.</returns>
    Task<ICollection<App.Domain.Delivery.DeliveryAttempt>> GetAllByDeliveryIdAsync(Guid deliveryId);

    /// <summary>
    /// Gets all delivery attempts for deliveries belonging to a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of delivery attempts belonging to the company.</returns>
    Task<ICollection<App.Domain.Delivery.DeliveryAttempt>> GetAllByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Gets a delivery attempt by ID including delivery information for tenant verification.
    /// </summary>
    /// <param name="id">The attempt ID.</param>
    /// <param name="companyId">The company ID for tenant scope.</param>
    /// <returns>The delivery attempt if found and belongs to the company.</returns>
    Task<App.Domain.Delivery.DeliveryAttempt?> GetByIdAsync(Guid id, Guid companyId);

    /// <summary>
    /// Gets all delivery attempts for a delivery within company scope.
    /// </summary>
    /// <param name="deliveryId">The delivery ID.</param>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of delivery attempts in company scope.</returns>
    Task<ICollection<App.Domain.Delivery.DeliveryAttempt>> GetAllByDeliveryIdAsync(Guid deliveryId, Guid companyId);
}
