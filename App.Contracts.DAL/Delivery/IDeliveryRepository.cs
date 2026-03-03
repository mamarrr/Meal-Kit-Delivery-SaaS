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
}
