using App.Contracts.BLL.Core;
using App.Contracts.DAL.Core;
using App.Domain.Core;

namespace App.BLL.Core;

public class CompanyService : BaseService<Company, ICompanyRepository>, ICompanyService
{
    public CompanyService(ICompanyRepository repository) : base(repository)
    {
    }
}

