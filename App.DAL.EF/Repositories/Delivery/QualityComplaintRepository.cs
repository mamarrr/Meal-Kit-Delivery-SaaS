using App.Contracts.DAL.Delivery;
using App.Domain.Delivery;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Delivery;

/// <summary>
/// EF Core repository implementation for QualityComplaint aggregate.
/// </summary>
public class QualityComplaintRepository : BaseRepository<QualityComplaint, AppDbContext>, IQualityComplaintRepository
{
    public QualityComplaintRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<QualityComplaint>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(qc => qc.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<QualityComplaint>> GetAllByCustomerIdAsync(Guid customerId, Guid companyId)
    {
        return await RepositoryDbSet
            .Include(qc => qc.QualityComplaintStatus)
            .Include(qc => qc.QualityComplaintType)
            .Include(qc => qc.Delivery)
            .Where(qc => qc.CustomerId == customerId && qc.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<QualityComplaint>> GetAllByCustomerIdAsync(Guid customerId)
    {
        return await RepositoryDbSet
            .Include(qc => qc.QualityComplaintStatus)
            .Include(qc => qc.QualityComplaintType)
            .Include(qc => qc.Delivery)
            .Where(qc => qc.CustomerId == customerId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> DeliveryBelongsToCustomerAsync(Guid deliveryId, Guid customerId)
    {
        return await RepositoryDbContext.Deliveries
            .AnyAsync(d => d.Id == deliveryId && d.CustomerId == customerId);
    }
}
