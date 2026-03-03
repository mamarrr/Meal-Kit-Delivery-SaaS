using App.Contracts.DAL.Menu;
using App.Domain.Menu;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Menu;

/// <summary>
/// EF Core repository implementation for WeeklyMenu aggregate.
/// </summary>
public class WeeklyMenuRepository : BaseRepository<WeeklyMenu, AppDbContext>, IWeeklyMenuRepository
{
    public WeeklyMenuRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<WeeklyMenu>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(wm => wm.CompanyId == companyId)
            .ToListAsync();
    }
}
