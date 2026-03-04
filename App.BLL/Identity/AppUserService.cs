using App.Contracts.BLL.Identity;
using App.Contracts.DAL.Identity;
using App.Domain.Identity;

namespace App.BLL.Identity;

public class AppUserService : BaseService<AppUser, IAppUserRepository>, IAppUserService
{
    public AppUserService(IAppUserRepository repository) : base(repository)
    {
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await Repository.GetByEmailAsync(email);
    }

    public async Task<AppUser?> GetByUserNameAsync(string userName)
    {
        return await Repository.GetByUserNameAsync(userName);
    }
}

