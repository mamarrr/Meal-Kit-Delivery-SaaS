using App.Domain.Menu;
using Contracts.DAL;

namespace App.Contracts.DAL.Menu;

public interface ICustomerPreferenceRepository : IRepository<CustomerPreference>
{
    Task<ICollection<CustomerPreference>> GetAllByCompanyIdAsync(Guid companyId);
    Task<CustomerPreference?> GetByIdAsync(Guid id, Guid companyId);
    Task<bool> IsCustomerInCompanyAsync(Guid customerId, Guid companyId);
    Task<bool> IsDietaryCategoryInCompanyAsync(Guid dietaryCategoryId, Guid companyId);
    
    /// <summary>
    /// Gets all customer preferences for a specific customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>A collection of customer preferences.</returns>
    Task<ICollection<CustomerPreference>> GetAllByCustomerIdAsync(Guid customerId);
}

