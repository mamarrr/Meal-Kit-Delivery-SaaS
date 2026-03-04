using App.Contracts.DAL.Support;
using App.Domain.Support;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Support;

public class TenantSupportAccessRepository : BaseRepository<TenantSupportAccess, AppDbContext>, ITenantSupportAccessRepository
{
    public TenantSupportAccessRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ICollection<TenantSupportAccess>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(x => x.CompanyId == companyId)
            .ToListAsync();
    }
}
