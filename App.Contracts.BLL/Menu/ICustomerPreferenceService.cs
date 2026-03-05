using App.Domain.Menu;

namespace App.Contracts.BLL.Menu;

public interface ICustomerPreferenceService : IBaseEntityService<CustomerPreference>
{
    Task<ICollection<CustomerPreference>> GetAllByCompanyIdAsync(Guid companyId);
    Task<CustomerPreference?> GetByIdAsync(Guid id, Guid companyId);
    Task<CustomerPreference> AddAsync(CustomerPreference entity, Guid companyId);
    Task<CustomerPreference> UpdateAsync(CustomerPreference entity, Guid companyId);
    Task<CustomerPreference> RemoveAsync(Guid id, Guid companyId);
    
    /// <summary>
    /// Gets all preferences for a specific customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>A collection of customer preferences.</returns>
    Task<ICollection<CustomerPreference>> GetAllByCustomerIdAsync(Guid customerId);
}

