using App.Domain.Menu;

namespace App.Contracts.BLL.Menu;

public interface IIngredientService : ITenantEntityService<Ingredient>
{
    Task<ICollection<IngredientCatalogItemDto>> GetCatalogAsync(Guid companyId);
    Task<IngredientCatalogItemDto> UpsertCatalogItemAsync(Guid companyId, Guid actorId, IngredientCatalogUpsertDto dto);
    Task RemoveCatalogItemAsync(Guid companyId, Guid ingredientId);
}

