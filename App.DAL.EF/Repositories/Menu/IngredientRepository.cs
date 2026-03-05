using App.Contracts.DAL.Menu;
using App.Domain.Menu;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Menu;

/// <summary>
/// EF Core repository implementation for Ingredient aggregate.
/// </summary>
public class IngredientRepository : BaseRepository<Ingredient, AppDbContext>, IIngredientRepository
{
    public IngredientRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<Ingredient>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(i => i.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<Ingredient>> GetAllByCompanyIdsAsync(IEnumerable<Guid> companyIds)
    {
        var ids = companyIds.ToList();
        if (ids.Count == 0)
        {
            return [];
        }

        return await RepositoryDbSet
            .Where(i => ids.Contains(i.CompanyId))
            .ToListAsync();
    }

    public async Task<Ingredient?> GetByNameAsync(Guid companyId, string name)
    {
        return await RepositoryDbSet
            .FirstOrDefaultAsync(i => i.CompanyId == companyId && i.Name == name);
    }

    public async Task<ICollection<Ingredient>> GetByIdsAsync(Guid companyId, ICollection<Guid> ids)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await RepositoryDbSet
            .Where(i => i.CompanyId == companyId && ids.Contains(i.Id))
            .ToListAsync();
    }
}
