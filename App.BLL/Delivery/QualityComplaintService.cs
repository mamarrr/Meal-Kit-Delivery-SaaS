using App.Contracts.BLL.Delivery;
using App.Contracts.DAL.Core;
using App.Contracts.DAL.Delivery;
using App.Domain.Delivery;

namespace App.BLL.Delivery;

public class QualityComplaintService : BaseTenantService<QualityComplaint, IQualityComplaintRepository>, IQualityComplaintService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDeliveryRepository _deliveryRepository;

    public QualityComplaintService(
        IQualityComplaintRepository repository,
        ICustomerRepository customerRepository,
        IDeliveryRepository deliveryRepository) : base(repository)
    {
        _customerRepository = customerRepository;
        _deliveryRepository = deliveryRepository;
    }

    protected override async Task<ICollection<QualityComplaint>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<ICollection<QualityComplaint>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId)
    {
        return await Repository.GetAllByCustomerIdAsync(customerId, companyId);
    }

    public async Task<ICollection<QualityComplaint>> GetAllByCustomerIdAsync(Guid customerId)
    {
        return await Repository.GetAllByCustomerIdAsync(customerId);
    }

    public async Task<QualityComplaint?> GetByCustomerIdAsync(Guid id, Guid customerId)
    {
        var complaint = await Repository.GetByIdAsync(id);
        return complaint != null && complaint.CustomerId == customerId ? complaint : null;
    }

    public override async Task<QualityComplaint> AddAsync(QualityComplaint entity, Guid companyId)
    {
        await ValidateTenantReferencesAsync(entity, companyId);
        return await base.AddAsync(entity, companyId);
    }

    public override async Task<QualityComplaint> UpdateAsync(QualityComplaint entity, Guid companyId)
    {
        await ValidateTenantReferencesAsync(entity, companyId);
        return await base.UpdateAsync(entity, companyId);
    }

    private async Task ValidateTenantReferencesAsync(QualityComplaint entity, Guid companyId)
    {
        var customer = await _customerRepository.GetByIdAsync(entity.CustomerId);
        var delivery = await _deliveryRepository.GetByIdAsync(entity.DeliveryId);

        var outOfScope = customer == null || customer.CompanyId != companyId
                         || delivery == null || delivery.CompanyId != companyId
                         || delivery.CustomerId != entity.CustomerId;

        if (outOfScope)
        {
            throw new KeyNotFoundException("Quality complaint references are outside company scope.");
        }
    }
}

