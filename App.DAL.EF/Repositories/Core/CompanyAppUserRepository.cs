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
    public async Task<ICollection<CompanyAppUser>> GetAllByAppUserIdAsync(Guid appUserId, Guid companyId)
    {
        return await RepositoryDbSet
            .Where(cau => cau.AppUserId == appUserId && cau.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<CompanyAppUser?> GetActiveMembershipAsync(Guid appUserId, Guid companyId)
    {
        return await RepositoryDbSet
            .Include(x => x.CompanyRole)
            .FirstOrDefaultAsync(x => x.AppUserId == appUserId && x.CompanyId == companyId && x.IsActive);
    }

    public override CompanyAppUser Update(CompanyAppUser entity)
    {
        var entry = RepositoryDbContext.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            RepositoryDbSet.Attach(entity);
            entry = RepositoryDbContext.Entry(entity);
        }

        // Mark only membership row as modified, avoid graph-wide updates to lookup/navigation entities.
        entry.State = EntityState.Modified;

        entry.Reference(x => x.CompanyRole).TargetEntry?.State = EntityState.Unchanged;
        entry.Reference(x => x.AppUser).TargetEntry?.State = EntityState.Unchanged;
        entry.Reference(x => x.Company).TargetEntry?.State = EntityState.Unchanged;
        entry.Reference(x => x.CreatedByAppUser).TargetEntry?.State = EntityState.Unchanged;

        return entity;
    }

    public async Task<ICollection<CompanyAppUser>> GetActiveMembershipsByCompanyAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Include(x => x.AppUser)
            .Include(x => x.CompanyRole)
            .Where(x => x.CompanyId == companyId && x.IsActive)
            .OrderByDescending(x => x.IsOwner)
            .ThenBy(x => x.AppUser!.Email)
            .ToListAsync();
    }

    public async Task<CompanyAppUser?> GetActiveOwnerByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Include(x => x.CompanyRole)
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.IsActive && x.IsOwner);
    }
}
