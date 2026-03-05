using App.Domain.Menu;
using Contracts.DAL;

namespace App.Contracts.DAL.Menu;

public interface ICustomerExclusionRepository : IRepository<CustomerExclusion>
{
    Task<ICollection<CustomerExclusion>> GetAllByCompanyIdAsync(Guid companyId);
    Task<CustomerExclusion?> GetByIdAsync(Guid id, Guid companyId);
    Task<bool> IsCustomerInCompanyAsync(Guid customerId, Guid companyId);
    Task<bool> IsIngredientInCompanyAsync(Guid ingredientId, Guid companyId);
    
    /// <summary>
    /// Gets all customer exclusions for a specific customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>A collection of customer exclusions.</returns>
    Task<ICollection<CustomerExclusion>> GetAllByCustomerIdAsync(Guid customerId);
}

