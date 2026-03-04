using App.Domain.Menu;
using Contracts.DAL;

namespace App.Contracts.DAL.Menu;

public interface ICustomerPreferenceRepository : IRepository<CustomerPreference>
{
    Task<ICollection<CustomerPreference>> GetAllByCompanyIdAsync(Guid companyId);
    Task<CustomerPreference?> GetByIdAsync(Guid id, Guid companyId);
    Task<bool> IsCustomerInCompanyAsync(Guid customerId, Guid companyId);
    Task<bool> IsDietaryCategoryInCompanyAsync(Guid dietaryCategoryId, Guid companyId);
}

