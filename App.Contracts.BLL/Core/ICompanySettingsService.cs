using App.Domain.Core;

namespace App.Contracts.BLL.Core;

public interface ICompanySettingsService : ITenantEntityService<CompanySettings>
{
    Task<CompanySettings?> GetByCompanyIdAsync(Guid companyId);
}

