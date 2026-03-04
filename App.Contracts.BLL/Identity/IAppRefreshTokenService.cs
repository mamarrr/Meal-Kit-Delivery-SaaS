using App.Domain.Identity;

namespace App.Contracts.BLL.Identity;

public interface IAppRefreshTokenService : IBaseEntityService<AppRefreshToken>
{
    Task<ICollection<AppRefreshToken>> GetAllByUserIdAsync(Guid userId);
    Task<AppRefreshToken?> GetByTokenAsync(string refreshToken);
}

