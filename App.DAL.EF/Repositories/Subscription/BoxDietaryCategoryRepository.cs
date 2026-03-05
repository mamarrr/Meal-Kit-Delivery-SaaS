using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Subscription;

public class BoxDietaryCategoryRepository : BaseRepository<BoxDietaryCategory, AppDbContext>, IBoxDietaryCategoryRepository
{
    public BoxDietaryCategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ICollection<BoxDietaryCategory>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(x => x.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<ICollection<BoxDietaryCategory>> GetAllByBoxIdAsync(Guid boxId, Guid companyId)
    {
        return await RepositoryDbSet
            .Where(x => x.BoxId == boxId && x.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task RemoveAllByBoxIdAsync(Guid boxId, Guid companyId)
    {
        var now = DateTime.UtcNow;
        var existing = await RepositoryDbSet
            .Where(x => x.BoxId == boxId && x.CompanyId == companyId && x.DeletedAt == null)
            .ToListAsync();

        foreach (var item in existing)
        {
            item.DeletedAt = now;
            RepositoryDbSet.Update(item);
        }
    }
}
