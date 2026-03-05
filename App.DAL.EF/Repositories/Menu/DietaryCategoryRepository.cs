using App.Contracts.DAL.Menu;
using App.Domain.Menu;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Menu;

/// <summary>
/// EF Core repository implementation for DietaryCategory aggregate.
/// </summary>
public class DietaryCategoryRepository : BaseRepository<DietaryCategory, AppDbContext>, IDietaryCategoryRepository
{
    public DietaryCategoryRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<DietaryCategory>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(dc => dc.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<DietaryCategory>> GetAllByCompanyIdsAsync(IEnumerable<Guid> companyIds)
    {
        var ids = companyIds.ToList();
        if (ids.Count == 0)
        {
            return [];
        }

        return await RepositoryDbSet
            .Where(dc => ids.Contains(dc.CompanyId))
            .ToListAsync();
    }
}
