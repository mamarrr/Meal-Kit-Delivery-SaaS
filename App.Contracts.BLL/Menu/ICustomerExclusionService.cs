using App.Domain.Menu;

namespace App.Contracts.BLL.Menu;

public interface ICustomerExclusionService : IBaseEntityService<CustomerExclusion>
{
    Task<ICollection<CustomerExclusion>> GetAllByCompanyIdAsync(Guid companyId);
    Task<CustomerExclusion?> GetByIdAsync(Guid id, Guid companyId);
    Task<CustomerExclusion> AddAsync(CustomerExclusion entity, Guid companyId);
    Task<CustomerExclusion> UpdateAsync(CustomerExclusion entity, Guid companyId);
    Task<CustomerExclusion> RemoveAsync(Guid id, Guid companyId);
    
    /// <summary>
    /// Gets all exclusions for a specific customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>A collection of customer exclusions.</returns>
    Task<ICollection<CustomerExclusion>> GetAllByCustomerIdAsync(Guid customerId);
}

