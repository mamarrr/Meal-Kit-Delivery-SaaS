using App.Domain.Delivery;

namespace App.Contracts.BLL.Delivery;

public interface IQualityComplaintService : ITenantEntityService<QualityComplaint>
{
    Task<ICollection<QualityComplaint>> GetAllByCustomerIdAsync(Guid customerId);
    Task<ICollection<QualityComplaint>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId);
    Task<QualityComplaint?> GetByIdAsync(Guid id, Guid customerId);
}

