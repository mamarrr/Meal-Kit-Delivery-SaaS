using App.Domain.Core;
using Contracts.DAL;

namespace App.Contracts.DAL.Core;

/// <summary>
/// Repository interface for Customer aggregate.
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// Gets all customers for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of customers belonging to the company.</returns>
    Task<ICollection<Customer>> GetAllByCompanyIdAsync(Guid companyId);
}
