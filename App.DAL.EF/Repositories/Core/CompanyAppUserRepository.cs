using App.Contracts.DAL.Core;
using App.Domain.Core;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Core;

/// <summary>
/// EF Core repository implementation for CompanyAppUser aggregate.
/// </summary>
public class CompanyAppUserRepository : BaseRepository<CompanyAppUser, AppDbContext>, ICompanyAppUserRepository
{
    public CompanyAppUserRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<CompanyAppUser>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(cau => cau.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<CompanyAppUser>> GetAllByAppUserIdAsync(Guid appUserId)
    {
        return await RepositoryDbSet
            .Where(cau => cau.AppUserId == appUserId)
            .ToListAsync();
    }
}
