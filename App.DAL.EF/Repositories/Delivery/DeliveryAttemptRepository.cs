using App.Contracts.DAL.Delivery;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Delivery;

/// <summary>
/// EF Core repository implementation for DeliveryAttempt aggregate.
/// </summary>
public class DeliveryAttemptRepository : BaseRepository<App.Domain.Delivery.DeliveryAttempt, AppDbContext>, IDeliveryAttemptRepository
{
    public DeliveryAttemptRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<App.Domain.Delivery.DeliveryAttempt>> GetAllByDeliveryIdAsync(Guid deliveryId)
    {
        return await RepositoryDbSet
            .Where(a => a.DeliveryId == deliveryId)
            .Include(a => a.DeliveryAttemptResult)
            .OrderBy(a => a.AttemptNo)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<App.Domain.Delivery.DeliveryAttempt>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(a => a.Delivery != null && a.Delivery.CompanyId == companyId)
            .Include(a => a.Delivery)
            .Include(a => a.DeliveryAttemptResult)
            .OrderByDescending(a => a.AttemptAt)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<App.Domain.Delivery.DeliveryAttempt?> GetByIdAsync(Guid id, Guid companyId)
    {
        return await RepositoryDbSet
            .Include(a => a.Delivery)
            .Include(a => a.DeliveryAttemptResult)
            .FirstOrDefaultAsync(a => a.Id == id && a.Delivery != null && a.Delivery.CompanyId == companyId);
    }

    /// <inheritdoc />
    public async Task<ICollection<App.Domain.Delivery.DeliveryAttempt>> GetAllByDeliveryIdAsync(Guid deliveryId, Guid companyId)
    {
        return await RepositoryDbSet
            .Where(a => a.DeliveryId == deliveryId && a.Delivery != null && a.Delivery.CompanyId == companyId)
            .Include(a => a.DeliveryAttemptResult)
            .OrderBy(a => a.AttemptNo)
            .ToListAsync();
    }
}
