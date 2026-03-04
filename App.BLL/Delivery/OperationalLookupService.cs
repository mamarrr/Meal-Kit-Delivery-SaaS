using App.Contracts.BLL.Delivery;
using App.DAL.EF;
using App.Domain.Delivery;
using Microsoft.EntityFrameworkCore;

namespace App.BLL.Delivery;

public class OperationalLookupService : IOperationalLookupService
{
    private readonly AppDbContext _context;

    public OperationalLookupService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<DeliveryStatus>> GetDeliveryStatusesAsync()
    {
        return await _context.DeliveryStatuses
            .OrderBy(x => x.Label)
            .ToListAsync();
    }

    public async Task<ICollection<DeliveryWindow>> GetDeliveryWindowsByCompanyIdAsync(Guid companyId)
    {
        var deliveryZoneIds = await _context.DeliveryZones
            .Where(z => z.CompanyId == companyId)
            .Select(z => z.Id)
            .ToListAsync();

        return await _context.DeliveryWindows
            .Where(w => deliveryZoneIds.Contains(w.DeliveryZoneId))
            .OrderBy(w => w.DayOfWeek)
            .ThenBy(w => w.StartTime)
            .ToListAsync();
    }

    public async Task<bool> DeliveryWindowBelongsToCompanyAsync(Guid deliveryWindowId, Guid companyId)
    {
        return await _context.DeliveryWindows
            .AnyAsync(w => w.Id == deliveryWindowId && w.DeliveryZone != null && w.DeliveryZone.CompanyId == companyId);
    }

    public async Task<ICollection<QualityComplaintType>> GetQualityComplaintTypesAsync()
    {
        return await _context.QualityComplaintTypes
            .OrderBy(x => x.Label)
            .ToListAsync();
    }

    public async Task<ICollection<QualityComplaintStatus>> GetQualityComplaintStatusesAsync()
    {
        return await _context.QualityComplaintStatuses
            .OrderBy(x => x.Label)
            .ToListAsync();
    }
}

