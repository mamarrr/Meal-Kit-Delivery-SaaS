using App.Contracts.BLL.Delivery;
using App.Contracts.DAL.Delivery;
using App.Domain.Delivery;

namespace App.BLL.Delivery;

/// <summary>
/// Service implementation for DeliveryAttempt operations.
/// </summary>
public class DeliveryAttemptService : BaseService<DeliveryAttempt, IDeliveryAttemptRepository>, IDeliveryAttemptService
{
    public DeliveryAttemptService(IDeliveryAttemptRepository repository) : base(repository)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<DeliveryAttempt>> GetAllByDeliveryIdAsync(Guid deliveryId)
    {
        return await Repository.GetAllByDeliveryIdAsync(deliveryId);
    }

    /// <inheritdoc />
    public async Task<ICollection<DeliveryAttempt>> GetAllByDeliveryIdAsync(Guid deliveryId, Guid companyId)
    {
        return await Repository.GetAllByDeliveryIdAsync(deliveryId, companyId);
    }

    /// <inheritdoc />
    public async Task<ICollection<DeliveryAttempt>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    /// <inheritdoc />
    public async Task<DeliveryAttempt?> GetByIdAndCompanyAsync(Guid id, Guid companyId)
    {
        return await Repository.GetByIdAsync(id, companyId);
    }
}
