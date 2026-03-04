using App.Contracts.BLL.Subscription;
using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;

namespace App.BLL.Subscription;

public class BoxPriceService : BaseTenantService<BoxPrice, IBoxPriceRepository>, IBoxPriceService
{
    public BoxPriceService(IBoxPriceRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<BoxPrice>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<ICollection<BoxPrice>> GetAllByBoxIdAsync(Guid boxId)
    {
        return await Repository.GetAllByBoxIdAsync(boxId);
    }

    public async Task<ICollection<BoxPrice>> GetActiveByBoxIdAsync(Guid boxId, Guid companyId)
    {
        return await Repository.GetActiveByBoxIdAsync(boxId, companyId);
    }
}
