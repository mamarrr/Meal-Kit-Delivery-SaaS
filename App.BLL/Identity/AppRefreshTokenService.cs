using App.Contracts.BLL.Identity;
using App.Contracts.DAL.Identity;
using App.Domain.Identity;

namespace App.BLL.Identity;

public class AppRefreshTokenService : BaseService<AppRefreshToken, IAppRefreshTokenRepository>, IAppRefreshTokenService
{
    public AppRefreshTokenService(IAppRefreshTokenRepository repository) : base(repository)
    {
    }

    public async Task<ICollection<AppRefreshToken>> GetAllByUserIdAsync(Guid userId)
    {
        return await Repository.GetAllByUserIdAsync(userId);
    }

    public async Task<AppRefreshToken?> GetByTokenAsync(string refreshToken)
    {
        return await Repository.GetByTokenAsync(refreshToken);
    }
}

