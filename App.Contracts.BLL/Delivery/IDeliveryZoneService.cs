using App.Domain.Delivery;

namespace App.Contracts.BLL.Delivery;

public interface IDeliveryZoneService : ITenantEntityService<DeliveryZone>
{
    Task<int?> GetMaxZonesForCompanyAsync(Guid companyId);

    Task<bool> CanCreateZoneAsync(Guid companyId);
}

