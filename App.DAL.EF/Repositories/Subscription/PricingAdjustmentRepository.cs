using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Subscription;

public class PricingAdjustmentRepository : BaseRepository<PricingAdjustment, AppDbContext>, IPricingAdjustmentRepository
{
    public PricingAdjustmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ICollection<PricingAdjustment>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.AdjustmentType)
            .ThenBy(x => x.Label)
            .ToListAsync();
    }

    public async Task<ICollection<PricingAdjustment>> GetAllByTypeAsync(Guid companyId, string adjustmentType)
    {
        return await RepositoryDbSet
            .Where(x => x.CompanyId == companyId && x.AdjustmentType == adjustmentType)
            .OrderBy(x => x.Label)
            .ToListAsync();
    }
}

