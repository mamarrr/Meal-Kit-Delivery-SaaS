using Contracts.DAL;

namespace App.Contracts.DAL.Delivery;

/// <summary>
/// Repository interface for Delivery aggregate.
/// </summary>
public interface IDeliveryRepository : IRepository<App.Domain.Delivery.Delivery>
{
    /// <summary>
    /// Gets all deliveries for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of deliveries belonging to the company.</returns>
    Task<ICollection<App.Domain.Delivery.Delivery>> GetAllByCompanyIdAsync(Guid companyId);
    
    /// <summary>
    /// Gets all deliveries for a specific customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>A collection of deliveries for the customer.</returns>
    Task<ICollection<App.Domain.Delivery.Delivery>> GetAllByCustomerIdAsync(Guid customerId);

    /// <summary>
    /// Checks whether a delivery belongs to a specific customer.
    /// </summary>
    /// <param name="deliveryId">The delivery ID.</param>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>True if the delivery belongs to the customer.</returns>
    Task<bool> DeliveryBelongsToCustomerAsync(Guid deliveryId, Guid customerId);

    /// <summary>
    /// Gets all deliveries for a specific customer within company scope.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of deliveries for the customer in the company.</returns>
    Task<ICollection<App.Domain.Delivery.Delivery>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId);

    /// <summary>
    /// Gets deliveries for a specific company and scheduled date (UTC date component).
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="scheduledDateUtc">Target UTC date.</param>
    /// <returns>A collection of deliveries scheduled on the specified date for the company.</returns>
    Task<ICollection<App.Domain.Delivery.Delivery>> GetAllByCompanyAndScheduledDateAsync(Guid companyId, DateTime scheduledDateUtc);
}
