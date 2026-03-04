using App.Contracts.BLL.Menu;
using App.Contracts.DAL.Menu;
using App.Domain.Menu;

namespace App.BLL.Menu;

public class WeeklyMenuService : BaseTenantService<WeeklyMenu, IWeeklyMenuRepository>, IWeeklyMenuService
{
    public WeeklyMenuService(IWeeklyMenuRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<WeeklyMenu>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }
}

