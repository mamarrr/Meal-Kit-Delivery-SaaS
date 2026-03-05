using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Subscription;

/// <summary>
/// EF Core repository implementation for BoxPrice aggregate.
/// </summary>
public class BoxPriceRepository : BaseRepository<BoxPrice, AppDbContext>, IBoxPriceRepository
{
    public BoxPriceRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<BoxPrice>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Include(bp => bp.Box)
            .Where(bp => bp.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<BoxPrice>> GetAllByBoxIdAsync(Guid boxId)
    {
        return await RepositoryDbSet
            .Where(bp => bp.BoxId == boxId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<BoxPrice>> GetActiveByBoxIdAsync(Guid boxId, Guid companyId)
    {
        var now = DateTime.UtcNow;
        return await RepositoryDbSet
            .Where(bp => bp.BoxId == boxId 
                      && bp.CompanyId == companyId 
                      && bp.IsActive
                      && (bp.ValidFrom == null || bp.ValidFrom <= now)
                      && (bp.ValidTo == null || bp.ValidTo >= now))
            .ToListAsync();
    }

    public async Task<ICollection<BoxPrice>> GetAllByCompanyIdWithDetailsAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Include(x => x.Box)
            .Where(x => x.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<BoxPrice?> GetByBoxAndPricingNameAsync(Guid boxId, string pricingName, Guid companyId)
    {
        return await RepositoryDbSet
            .FirstOrDefaultAsync(x => x.BoxId == boxId
                                      && x.PricingName == pricingName
                                      && x.CompanyId == companyId);
    }
}
