using App.Contracts.DAL.Core;
using App.Domain.Core;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Core;

/// <summary>
/// EF Core repository implementation for CompanySettings aggregate.
/// </summary>
public class CompanySettingsRepository : BaseRepository<CompanySettings, AppDbContext>, ICompanySettingsRepository
{
    public CompanySettingsRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<CompanySettings?> GetByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .FirstOrDefaultAsync(cs => cs.CompanyId == companyId);
    }
}
