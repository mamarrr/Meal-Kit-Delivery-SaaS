using App.Contracts.DAL.Delivery;
using App.Domain.Delivery;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Delivery;

/// <summary>
/// EF Core repository implementation for DeliveryZone aggregate.
/// </summary>
public class DeliveryZoneRepository : BaseRepository<DeliveryZone, AppDbContext>, IDeliveryZoneRepository
{
    public DeliveryZoneRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<DeliveryZone>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(dz => dz.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<DeliveryZone>> GetActiveByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(dz => dz.CompanyId == companyId && dz.IsActive && dz.DeletedAt == null)
            .ToListAsync();
    }
}
