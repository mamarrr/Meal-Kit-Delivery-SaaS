using App.Domain.Delivery;
using Contracts.DAL;

namespace App.Contracts.DAL.Delivery;

/// <summary>
/// Repository interface for QualityComplaint aggregate.
/// </summary>
public interface IQualityComplaintRepository : IRepository<QualityComplaint>
{
    /// <summary>
    /// Gets all quality complaints for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of quality complaints belonging to the company.</returns>
    Task<ICollection<QualityComplaint>> GetAllByCompanyIdAsync(Guid companyId);
    
    /// <summary>
    /// Gets all quality complaints for a specific customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>A collection of quality complaints for the customer.</returns>
    Task<ICollection<QualityComplaint>> GetAllByCustomerIdAsync(Guid customerId);
}
