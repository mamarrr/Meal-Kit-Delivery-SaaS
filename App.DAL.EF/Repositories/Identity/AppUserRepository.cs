using App.Contracts.DAL.Identity;
using App.Domain.Identity;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Identity;

/// <summary>
/// EF Core repository implementation for AppUser aggregate.
/// </summary>
public class AppUserRepository : BaseRepository<AppUser, AppDbContext>, IAppUserRepository
{
    public AppUserRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToUpperInvariant();

        return await RepositoryDbSet
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
    }

    /// <inheritdoc />
    public async Task<AppUser?> GetByUserNameAsync(string userName)
    {
        return await RepositoryDbSet
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }
}
