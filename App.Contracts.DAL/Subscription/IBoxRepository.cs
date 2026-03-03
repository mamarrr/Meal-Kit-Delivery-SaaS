using App.Domain.Subscription;
using Contracts.DAL;

namespace App.Contracts.DAL.Subscription;

/// <summary>
/// Repository interface for Box aggregate.
/// </summary>
public interface IBoxRepository : IRepository<Box>
{
    /// <summary>
    /// Gets all boxes for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of boxes belonging to the company.</returns>
    Task<ICollection<Box>> GetAllByCompanyIdAsync(Guid companyId);
}
