using App.Contracts.BLL.Menu;
using App.Contracts.DAL.Menu;
using App.Domain.Menu;

namespace App.BLL.Menu;

public class IngredientService : BaseTenantService<Ingredient, IIngredientRepository>, IIngredientService
{
    public IngredientService(IIngredientRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<Ingredient>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }
}

