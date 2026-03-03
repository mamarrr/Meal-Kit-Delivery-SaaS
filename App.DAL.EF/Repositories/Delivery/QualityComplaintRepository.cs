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
    public async Task<ICollection<QualityComplaint>> GetAllByCustomerIdAsync(Guid customerId)
    {
        return await RepositoryDbSet
            .Where(qc => qc.CustomerId == customerId)
            .ToListAsync();
    }
}
