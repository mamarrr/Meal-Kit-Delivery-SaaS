using App.Contracts.BLL.Core;
using App.Contracts.DAL.Core;
using App.Domain.Core;

namespace App.BLL.Core;

public class CompanySettingsService : BaseTenantService<CompanySettings, ICompanySettingsRepository>, ICompanySettingsService
{
    public CompanySettingsService(ICompanySettingsRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<CompanySettings>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        var settings = await Repository.GetByCompanyIdAsync(companyId);
        return settings == null ? [] : [settings];
    }

    public async Task<CompanySettings?> GetByCompanyIdAsync(Guid companyId)
    {
        return await Repository.GetByCompanyIdAsync(companyId);
    }
}

