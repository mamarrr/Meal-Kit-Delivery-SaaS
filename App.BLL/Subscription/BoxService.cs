using App.Contracts.BLL.Subscription;
using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;

namespace App.BLL.Subscription;

public class BoxService : BaseTenantService<Box, IBoxRepository>, IBoxService
{
    public BoxService(IBoxRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<Box>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }
}

