using App.Domain.Menu;

namespace App.Contracts.BLL.Menu;

public interface IDietaryCategoryService : ITenantEntityService<DietaryCategory>
{
    Task<ICollection<DietaryCategoryCatalogItemDto>> GetCatalogAsync(Guid companyId);
    Task<DietaryCategoryCatalogItemDto> UpsertCatalogItemAsync(Guid companyId, Guid actorId, DietaryCategoryCatalogUpsertDto dto);
    Task RemoveCatalogItemAsync(Guid companyId, Guid dietaryCategoryId);
    
    /// <summary>
    /// Gets dietary categories from multiple companies.
    /// </summary>
    /// <param name="companyIds">The company IDs to get categories from.</param>
    /// <returns>A collection of dietary categories from the specified companies.</returns>
    Task<ICollection<DietaryCategory>> GetAllByCompanyIdsAsync(IEnumerable<Guid> companyIds);
}

