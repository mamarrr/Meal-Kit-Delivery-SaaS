using App.Domain.Menu;

namespace App.Contracts.BLL.Menu;

public interface ICustomerExclusionService : IBaseEntityService<CustomerExclusion>
{
    Task<ICollection<CustomerExclusion>> GetAllByCompanyIdAsync(Guid companyId);
    Task<CustomerExclusion?> GetByIdAsync(Guid id, Guid companyId);
    Task<CustomerExclusion> AddAsync(CustomerExclusion entity, Guid companyId);
    Task<CustomerExclusion> UpdateAsync(CustomerExclusion entity, Guid companyId);
    Task<CustomerExclusion> RemoveAsync(Guid id, Guid companyId);
}

