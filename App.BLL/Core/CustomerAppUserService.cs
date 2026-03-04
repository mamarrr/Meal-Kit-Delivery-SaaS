using App.Contracts.BLL.Core;
using App.Contracts.DAL.Core;
using App.Domain.Core;

namespace App.BLL.Core;

public class CustomerAppUserService : BaseService<CustomerAppUser, ICustomerAppUserRepository>, ICustomerAppUserService
{
    public CustomerAppUserService(ICustomerAppUserRepository repository) : base(repository)
    {
    }

    public async Task<ICollection<CustomerAppUser>> GetAllByCustomerIdAsync(Guid customerId)
    {
        return await Repository.GetAllByCustomerIdAsync(customerId);
    }

    public async Task<ICollection<CustomerAppUser>> GetAllByAppUserIdAsync(Guid appUserId)
    {
        return await Repository.GetAllByAppUserIdAsync(appUserId);
    }
}

