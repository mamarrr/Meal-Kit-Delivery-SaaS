using App.Domain.Delivery;

namespace App.Contracts.BLL.Delivery;

public interface IDeliveryService : ITenantEntityService<App.Domain.Delivery.Delivery>
{
    Task<ICollection<App.Domain.Delivery.Delivery>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId);

    Task<ICollection<App.Domain.Delivery.Delivery>> GetAllByCompanyAndScheduledDateAsync(Guid companyId, DateTime scheduledDateUtc);
}

