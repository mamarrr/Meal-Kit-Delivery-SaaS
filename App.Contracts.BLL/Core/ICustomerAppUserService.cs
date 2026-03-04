using App.Domain.Core;

namespace App.Contracts.BLL.Core;

public interface ICustomerAppUserService : IBaseEntityService<CustomerAppUser>
{
    Task<ICollection<CustomerAppUser>> GetAllByCustomerIdAsync(Guid customerId);
    Task<ICollection<CustomerAppUser>> GetAllByAppUserIdAsync(Guid appUserId);
}

