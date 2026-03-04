using App.Contracts.BLL.Menu;
using App.Contracts.DAL.Menu;
using App.Domain.Menu;

namespace App.BLL.Menu;

public class DietaryCategoryService : BaseTenantService<DietaryCategory, IDietaryCategoryRepository>, IDietaryCategoryService
{
    public DietaryCategoryService(IDietaryCategoryRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<DietaryCategory>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }
}

