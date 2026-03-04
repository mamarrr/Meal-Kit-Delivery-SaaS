using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Subscription;

/// <summary>
/// EF Core repository implementation for Box aggregate.
/// </summary>
public class BoxRepository : BaseRepository<Box, AppDbContext>, IBoxRepository
{
    public BoxRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<Box>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(b => b.CompanyId == companyId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<int> CountActiveByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(b => b.CompanyId == companyId && b.DeletedAt == null)
            .CountAsync();
    }
}
