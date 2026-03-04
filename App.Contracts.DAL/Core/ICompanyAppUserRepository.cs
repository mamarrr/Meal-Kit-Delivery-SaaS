using App.Domain.Core;
using Contracts.DAL;

namespace App.Contracts.DAL.Core;

/// <summary>
/// Repository interface for CompanyAppUser aggregate.
/// </summary>
public interface ICompanyAppUserRepository : IRepository<CompanyAppUser>
{
    /// <summary>
    /// Gets all company-app-user relationships for a specific company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of company-app-user relationships.</returns>
    Task<ICollection<CompanyAppUser>> GetAllByCompanyIdAsync(Guid companyId);
    
    /// <summary>
    /// Gets all company-app-user relationships for a specific app user within company scope.
    /// </summary>
    /// <param name="appUserId">The app user ID.</param>
    /// <param name="companyId">The company ID.</param>
    /// <returns>A collection of company-app-user relationships.</returns>
    Task<ICollection<CompanyAppUser>> GetAllByAppUserIdAsync(Guid appUserId, Guid companyId);
}
