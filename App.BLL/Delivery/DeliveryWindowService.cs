using App.Contracts.BLL.Delivery;
using App.Contracts.DAL.Delivery;
using App.Domain.Delivery;

namespace App.BLL.Delivery;

public class DeliveryWindowService : BaseService<DeliveryWindow, IDeliveryWindowRepository>, IDeliveryWindowService
{
    public DeliveryWindowService(IDeliveryWindowRepository repository) : base(repository)
    {
    }

    public async Task<ICollection<DeliveryWindow>> GetAllByDeliveryZoneIdAsync(Guid deliveryZoneId)
    {
        return await Repository.GetAllByDeliveryZoneIdAsync(deliveryZoneId);
    }

    public async Task<ICollection<DeliveryWindow>> GetActiveByDeliveryZoneIdAsync(Guid deliveryZoneId)
    {
        return await Repository.GetActiveByDeliveryZoneIdAsync(deliveryZoneId);
    }

    public async Task<ICollection<DeliveryWindow>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }
}
