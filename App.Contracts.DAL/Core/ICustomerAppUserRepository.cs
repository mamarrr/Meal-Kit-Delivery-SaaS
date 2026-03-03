using App.Domain.Core;
using Contracts.DAL;

namespace App.Contracts.DAL.Core;

/// <summary>
/// Repository interface for CustomerAppUser aggregate.
/// </summary>
public interface ICustomerAppUserRepository : IRepository<CustomerAppUser>
{
    /// <summary>
    /// Gets all customer-app-user relationships for a specific customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>A collection of customer-app-user relationships.</returns>
    Task<ICollection<CustomerAppUser>> GetAllByCustomerIdAsync(Guid customerId);
    
    /// <summary>
    /// Gets all customer-app-user relationships for a specific app user.
    /// </summary>
    /// <param name="appUserId">The app user ID.</param>
    /// <returns>A collection of customer-app-user relationships.</returns>
    Task<ICollection<CustomerAppUser>> GetAllByAppUserIdAsync(Guid appUserId);
}
