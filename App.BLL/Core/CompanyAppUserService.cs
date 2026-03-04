using App.Contracts.BLL.Core;
using App.Contracts.DAL.Core;
using App.Domain.Core;

namespace App.BLL.Core;

public class CompanyAppUserService : BaseTenantService<CompanyAppUser, ICompanyAppUserRepository>, ICompanyAppUserService
{
    public CompanyAppUserService(ICompanyAppUserRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<CompanyAppUser>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<ICollection<CompanyAppUser>> GetAllByAppUserIdAsync(Guid appUserId, Guid companyId)
    {
        return await Repository.GetAllByAppUserIdAsync(appUserId, companyId);
    }
}

