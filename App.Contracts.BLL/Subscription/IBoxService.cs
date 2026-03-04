using App.Domain.Subscription;

namespace App.Contracts.BLL.Subscription;

public interface IBoxService : ITenantEntityService<Box>
{
    /// <summary>
    /// Counts active (non-deleted) boxes for a company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <returns>Number of active boxes.</returns>
    Task<int> CountActiveByCompanyIdAsync(Guid companyId);
}

