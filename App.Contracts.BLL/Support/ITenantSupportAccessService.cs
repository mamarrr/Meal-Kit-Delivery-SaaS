using App.Domain.Support;

namespace App.Contracts.BLL.Support;

public interface ITenantSupportAccessService : IBaseEntityService<TenantSupportAccess>
{
    Task<ICollection<TenantSupportAccess>> GetAllByCompanyIdAsync(Guid companyId);
}
