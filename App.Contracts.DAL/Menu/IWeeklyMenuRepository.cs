using App.Domain.Menu;
using Contracts.DAL;

namespace App.Contracts.DAL.Menu;

/// <summary>
/// Repository interface for WeeklyMenu aggregate.
/// </summary>
public interface IWeeklyMenuRepository : IRepository<WeeklyMenu>
{
    /// <summary>
    /// Gets all weekly menus for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of weekly menus belonging to the company.</returns>
    Task<ICollection<WeeklyMenu>> GetAllByCompanyIdAsync(Guid companyId);
}
