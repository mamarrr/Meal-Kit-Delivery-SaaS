using App.Contracts.BLL.Menu;
using App.Contracts.DAL.Menu;
using App.Domain.Menu;

namespace App.BLL.Menu;

public class RecipeService : BaseTenantService<Recipe, IRecipeRepository>, IRecipeService
{
    public RecipeService(IRecipeRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<Recipe>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }
}

