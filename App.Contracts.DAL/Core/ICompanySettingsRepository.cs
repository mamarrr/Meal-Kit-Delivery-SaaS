using App.Domain.Core;
using Contracts.DAL;

namespace App.Contracts.DAL.Core;

/// <summary>
/// Repository interface for CompanySettings aggregate.
/// </summary>
public interface ICompanySettingsRepository : IRepository<CompanySettings>
{
    /// <summary>
    /// Gets the settings for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>The company settings, or null if not found.</returns>
    Task<CompanySettings?> GetByCompanyIdAsync(Guid companyId);
}
