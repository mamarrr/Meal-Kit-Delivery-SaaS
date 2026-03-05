using App.Domain.Delivery;

namespace App.Contracts.BLL.Delivery;

public interface IOperationalLookupService
{
    Task<ICollection<DeliveryStatus>> GetDeliveryStatusesAsync();

    Task<ICollection<DeliveryAttemptResult>> GetDeliveryAttemptResultsAsync();

    Task<ICollection<DeliveryWindow>> GetDeliveryWindowsByCompanyIdAsync(Guid companyId);

    Task<bool> DeliveryWindowBelongsToCompanyAsync(Guid deliveryWindowId, Guid companyId);

    Task<ICollection<QualityComplaintType>> GetQualityComplaintTypesAsync();

    Task<ICollection<QualityComplaintStatus>> GetQualityComplaintStatusesAsync();
}

