using App.Contracts.BLL.Delivery;
using App.Contracts.DAL.Delivery;
using App.Domain.Delivery;

namespace App.BLL.Delivery;

public class DeliveryZoneService : BaseTenantService<DeliveryZone, IDeliveryZoneRepository>, IDeliveryZoneService
{
    public DeliveryZoneService(IDeliveryZoneRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<DeliveryZone>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }
}

