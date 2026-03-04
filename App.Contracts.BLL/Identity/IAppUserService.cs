using App.Domain.Identity;

namespace App.Contracts.BLL.Identity;

public interface IAppUserService : IBaseEntityService<AppUser>
{
    Task<AppUser?> GetByEmailAsync(string email);
    Task<AppUser?> GetByUserNameAsync(string userName);
}

