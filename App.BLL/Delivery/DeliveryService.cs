using App.Contracts.BLL.Delivery;
using App.Contracts.DAL.Delivery;
using DeliveryEntity = App.Domain.Delivery.Delivery;

namespace App.BLL.Delivery;

public class DeliveryService : BaseTenantService<DeliveryEntity, IDeliveryRepository>, IDeliveryService
{
    public DeliveryService(IDeliveryRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<DeliveryEntity>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<ICollection<DeliveryEntity>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId)
    {
        return await Repository.GetAllByCustomerIdAsync(customerId, companyId);
    }
}

