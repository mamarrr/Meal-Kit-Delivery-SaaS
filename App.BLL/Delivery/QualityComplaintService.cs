using App.Contracts.BLL.Delivery;
using App.Contracts.DAL.Delivery;
using App.Domain.Delivery;

namespace App.BLL.Delivery;

public class QualityComplaintService : BaseTenantService<QualityComplaint, IQualityComplaintRepository>, IQualityComplaintService
{
    public QualityComplaintService(IQualityComplaintRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<QualityComplaint>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<ICollection<QualityComplaint>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId)
    {
        return await Repository.GetAllByCustomerIdAsync(customerId, companyId);
    }
}

