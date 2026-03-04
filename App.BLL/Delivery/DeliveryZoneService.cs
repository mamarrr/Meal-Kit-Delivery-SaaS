using App.Contracts.BLL.Delivery;
using App.Contracts.DAL.Delivery;
using App.Contracts.BLL.Subscription;
using App.Domain.Delivery;

namespace App.BLL.Delivery;

public class DeliveryZoneService : BaseTenantService<DeliveryZone, IDeliveryZoneRepository>, IDeliveryZoneService
{
    private readonly IPlatformSubscriptionService _platformSubscriptionService;

    public DeliveryZoneService(
        IDeliveryZoneRepository repository,
        IPlatformSubscriptionService platformSubscriptionService) : base(repository)
    {
        _platformSubscriptionService = platformSubscriptionService;
    }

    protected override async Task<ICollection<DeliveryZone>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<int?> GetMaxZonesForCompanyAsync(Guid companyId)
    {
        var activeSubscription = await _platformSubscriptionService.GetCurrentActiveByCompanyIdAsync(companyId);
        return activeSubscription?.PlatformSubscriptionTier?.MaxZones;
    }

    public async Task<bool> CanCreateZoneAsync(Guid companyId)
    {
        var maxZones = await GetMaxZonesForCompanyAsync(companyId);
        if (!maxZones.HasValue)
        {
            return true;
        }

        var existingZones = await Repository.GetAllByCompanyIdAsync(companyId);
        var activeZoneCount = existingZones.Count(z => z.DeletedAt == null);
        return activeZoneCount < maxZones.Value;
    }
}

