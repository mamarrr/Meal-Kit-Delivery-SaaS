using App.Contracts.DAL.Identity;
using App.Domain.Identity;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Identity;

/// <summary>
/// EF Core repository implementation for AppRefreshToken aggregate.
/// </summary>
public class AppRefreshTokenRepository : BaseRepository<AppRefreshToken, AppDbContext>, IAppRefreshTokenRepository
{
    public AppRefreshTokenRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<AppRefreshToken>> GetAllByUserIdAsync(Guid userId)
    {
        return await RepositoryDbSet
            .Where(rt => rt.UserId == userId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<AppRefreshToken?> GetByTokenAsync(string refreshToken)
    {
        return await RepositoryDbSet
            .FirstOrDefaultAsync(rt => rt.RefreshToken == refreshToken);
    }
}
