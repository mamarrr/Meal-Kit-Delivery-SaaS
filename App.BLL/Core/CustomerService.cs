using App.Contracts.BLL.Core;
using App.Contracts.DAL.Core;
using App.Domain.Core;

namespace App.BLL.Core;

public class CustomerService : BaseTenantService<Customer, ICustomerRepository>, ICustomerService
{
    public CustomerService(ICustomerRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<Customer>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }
}

