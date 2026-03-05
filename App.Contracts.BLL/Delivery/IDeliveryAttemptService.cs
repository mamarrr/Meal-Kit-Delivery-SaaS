using App.Domain.Delivery;

namespace App.Contracts.BLL.Delivery;

/// <summary>
/// Service interface for DeliveryAttempt operations.
/// </summary>
public interface IDeliveryAttemptService : IBaseEntityService<DeliveryAttempt>
{
    /// <summary>
    /// Gets all delivery attempts for a specific delivery.
    /// </summary>
    Task<ICollection<DeliveryAttempt>> GetAllByDeliveryIdAsync(Guid deliveryId);

    /// <summary>
    /// Gets all delivery attempts for a specific delivery within company scope.
    /// </summary>
    Task<ICollection<DeliveryAttempt>> GetAllByDeliveryIdAsync(Guid deliveryId, Guid companyId);

    /// <summary>
    /// Gets all delivery attempts for deliveries belonging to a specific company.
    /// </summary>
    Task<ICollection<DeliveryAttempt>> GetAllByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Gets a delivery attempt by ID with tenant verification.
    /// </summary>
    Task<DeliveryAttempt?> GetByIdAndCompanyAsync(Guid id, Guid companyId);
}
