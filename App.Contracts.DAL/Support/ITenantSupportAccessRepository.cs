using App.Domain.Support;
using Contracts.DAL;

namespace App.Contracts.DAL.Support;

public interface ITenantSupportAccessRepository : IRepository<TenantSupportAccess>
{
    Task<ICollection<TenantSupportAccess>> GetAllByCompanyIdAsync(Guid companyId);
}
