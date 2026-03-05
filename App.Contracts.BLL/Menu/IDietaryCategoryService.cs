using App.Domain.Menu;

namespace App.Contracts.BLL.Menu;

public interface IDietaryCategoryService : ITenantEntityService<DietaryCategory>
{
    Task<ICollection<DietaryCategoryCatalogItemDto>> GetCatalogAsync(Guid companyId);
    Task<DietaryCategoryCatalogItemDto> UpsertCatalogItemAsync(Guid companyId, Guid actorId, DietaryCategoryCatalogUpsertDto dto);
}

