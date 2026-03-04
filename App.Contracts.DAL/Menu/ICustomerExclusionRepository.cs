using App.Domain.Menu;
using Contracts.DAL;

namespace App.Contracts.DAL.Menu;

public interface ICustomerExclusionRepository : IRepository<CustomerExclusion>
{
    Task<ICollection<CustomerExclusion>> GetAllByCompanyIdAsync(Guid companyId);
    Task<CustomerExclusion?> GetByIdAsync(Guid id, Guid companyId);
    Task<bool> IsCustomerInCompanyAsync(Guid customerId, Guid companyId);
    Task<bool> IsIngredientInCompanyAsync(Guid ingredientId, Guid companyId);
}

