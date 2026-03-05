using App.Domain.Subscription;
using Contracts.DAL;

namespace App.Contracts.DAL.Subscription;

public interface IPricingAdjustmentRepository : IRepository<PricingAdjustment>
{
    Task<ICollection<PricingAdjustment>> GetAllByCompanyIdAsync(Guid companyId);
    Task<ICollection<PricingAdjustment>> GetAllByTypeAsync(Guid companyId, string adjustmentType);
}

