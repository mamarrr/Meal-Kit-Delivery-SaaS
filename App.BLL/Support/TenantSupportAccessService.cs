using App.Contracts.BLL.Support;
using App.Contracts.DAL.Support;
using App.Domain.Support;

namespace App.BLL.Support;

public class TenantSupportAccessService : BaseService<TenantSupportAccess, ITenantSupportAccessRepository>, ITenantSupportAccessService
{
    public TenantSupportAccessService(ITenantSupportAccessRepository repository) : base(repository)
    {
    }

    public async Task<ICollection<TenantSupportAccess>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }
}
