using App.Contracts.DAL.Delivery;
using App.Domain.Delivery;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Delivery;

/// <summary>
/// EF Core repository implementation for DeliveryWindow aggregate.
/// </summary>
public class DeliveryWindowRepository : BaseRepository<DeliveryWindow, AppDbContext>, IDeliveryWindowRepository
{
    public DeliveryWindowRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<DeliveryWindow>> GetAllByDeliveryZoneIdAsync(Guid deliveryZoneId)
    {
        return await RepositoryDbSet
            .Where(dw => dw.DeliveryZoneId == deliveryZoneId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<DeliveryWindow>> GetActiveByDeliveryZoneIdAsync(Guid deliveryZoneId)
    {
        return await RepositoryDbSet
            .Where(dw => dw.DeliveryZoneId == deliveryZoneId && dw.IsActive)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<DeliveryWindow>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Include(dw => dw.DeliveryZone)
            .Where(dw => dw.DeliveryZone != null && dw.DeliveryZone.CompanyId == companyId)
            .ToListAsync();
    }
}
