using App.Domain.Menu;

namespace App.Contracts.BLL.Menu;

public interface IIngredientService : ITenantEntityService<Ingredient>
{
    Task<ICollection<IngredientCatalogItemDto>> GetCatalogAsync(Guid companyId);
    Task<IngredientCatalogItemDto> UpsertCatalogItemAsync(Guid companyId, Guid actorId, IngredientCatalogUpsertDto dto);
    Task RemoveCatalogItemAsync(Guid companyId, Guid ingredientId);
    
    /// <summary>
    /// Gets ingredients from multiple companies.
    /// </summary>
    /// <param name="companyIds">The company IDs to get ingredients from.</param>
    /// <returns>A collection of ingredients from the specified companies.</returns>
    Task<ICollection<Ingredient>> GetAllByCompanyIdsAsync(IEnumerable<Guid> companyIds);
}

