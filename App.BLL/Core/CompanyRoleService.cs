using App.Contracts.BLL.Core;
using App.Contracts.DAL.Core;
using App.Domain.Core;

namespace App.BLL.Core;

public class CompanyRoleService : BaseService<CompanyRole, ICompanyRoleRepository>, ICompanyRoleService
{
    public CompanyRoleService(ICompanyRoleRepository repository) : base(repository)
    {
    }
}

