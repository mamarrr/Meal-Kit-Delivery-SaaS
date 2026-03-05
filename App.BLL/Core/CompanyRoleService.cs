using App.Contracts.BLL.Core;
using App.Contracts.DAL.Core;
using App.Domain.Core;

namespace App.BLL.Core;

public class CompanyRoleService : BaseService<CompanyRole, ICompanyRoleRepository>, ICompanyRoleService
{
    public CompanyRoleService(ICompanyRoleRepository repository) : base(repository)
    {
    }

    public async Task<CompanyRole?> GetByCodeAsync(string code)
    {
        return await Repository.GetByCodeAsync(code);
    }

    public async Task<ICollection<CompanyRole>> GetAllAssignableAsync()
    {
        return await Repository.GetAllAssignableAsync();
    }
}

