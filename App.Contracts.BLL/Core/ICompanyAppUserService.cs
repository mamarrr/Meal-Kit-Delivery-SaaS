using App.Domain.Core;

namespace App.Contracts.BLL.Core;

public interface ICompanyAppUserService : ITenantEntityService<CompanyAppUser>
{
    Task<ICollection<CompanyAppUser>> GetAllByAppUserIdAsync(Guid appUserId, Guid companyId);
}

