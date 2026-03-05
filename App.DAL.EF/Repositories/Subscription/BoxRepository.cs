using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Subscription;

/// <summary>
/// EF Core repository implementation for Box aggregate.
/// </summary>
public class BoxRepository : BaseRepository<Box, AppDbContext>, IBoxRepository
{
    public BoxRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<Box>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(b => b.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<int> CountActiveByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(b => b.CompanyId == companyId && b.DeletedAt == null)
            .CountAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<Box>> GetDiscoverableBoxesAsync(
        IReadOnlyCollection<Guid>? companyIds,
        decimal? minPrice,
        decimal? maxPrice,
        IReadOnlyCollection<Guid>? dietaryCategoryIds)
    {
        var now = DateTime.UtcNow;

        var query = RepositoryDbSet
            .AsQueryable()
            .Include(b => b.Company)
            .Include(b => b.BoxPrices)
            .Include(b => b.BoxDietaryCategories)
            .Where(b => b.IsActive && b.DeletedAt == null);

        if (companyIds is { Count: > 0 })
        {
            query = query.Where(b => companyIds.Contains(b.CompanyId));
        }

        if (dietaryCategoryIds is { Count: > 0 })
        {
            query = query.Where(b => b.BoxDietaryCategories != null
                                     && b.BoxDietaryCategories.Any(dc => dc.DeletedAt == null
                                                                          && dietaryCategoryIds.Contains(dc.DietaryCategoryId)));
        }

        if (minPrice.HasValue || maxPrice.HasValue)
        {
            query = query.Where(b => b.BoxPrices != null
                                     && b.BoxPrices.Any(bp => bp.DeletedAt == null
                                                               && bp.IsActive
                                                               && (bp.ValidFrom == null || bp.ValidFrom <= now)
                                                               && (bp.ValidTo == null || bp.ValidTo >= now)
                                                               && (!minPrice.HasValue || bp.PriceAmount >= minPrice.Value)
                                                               && (!maxPrice.HasValue || bp.PriceAmount <= maxPrice.Value)));
        }

        return await query.ToListAsync();
    }
}
