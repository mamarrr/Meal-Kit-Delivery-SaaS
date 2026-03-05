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

    public async Task<ICollection<IngredientCatalogItemDto>> GetCatalogAsync(Guid companyId)
    {
        var rows = await Repository.GetAllByCompanyIdAsync(companyId);

        return rows
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.Name)
            .Select(x => new IngredientCatalogItemDto
            {
                IngredientId = x.Id,
                Name = x.Name,
                NormalizedName = x.NormalizedName ?? NormalizeName(x.Name),
                IsAllergen = x.IsAllergen,
                IsExclusionTag = x.IsExclusionTag,
                ExclusionKey = x.ExclusionKey
            })
            .ToList();
    }

    public async Task<IngredientCatalogItemDto> UpsertCatalogItemAsync(Guid companyId, Guid actorId, IngredientCatalogUpsertDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentException("Ingredient name is required.", nameof(dto));
        }

        var normalizedName = NormalizeName(dto.Name);
        var exclusionKey = normalizedName;

        if (dto.IngredientId.HasValue)
        {
            var existing = await GetByIdAsync(dto.IngredientId.Value, companyId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Ingredient {dto.IngredientId.Value} was not found in company scope {companyId}.");
            }

            var duplicate = await Repository.GetByNameAsync(companyId, dto.Name.Trim());
            if (duplicate != null && duplicate.Id != existing.Id && duplicate.DeletedAt == null)
            {
                throw new InvalidOperationException("Ingredient name must be unique within company scope.");
            }

            existing.Name = dto.Name.Trim();
            existing.NormalizedName = normalizedName;
            existing.IsAllergen = dto.IsAllergen;
            existing.IsExclusionTag = true;
            existing.ExclusionKey = exclusionKey;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await UpdateAsync(existing, companyId);
            return ToCatalogItem(updated);
        }

        var alreadyExists = await Repository.GetByNameAsync(companyId, dto.Name.Trim());
        if (alreadyExists != null && alreadyExists.DeletedAt == null)
        {
            throw new InvalidOperationException("Ingredient name must be unique within company scope.");
        }

        if (alreadyExists != null)
        {
            alreadyExists.DeletedAt = null;
            alreadyExists.Name = dto.Name.Trim();
            alreadyExists.NormalizedName = normalizedName;
            alreadyExists.IsAllergen = dto.IsAllergen;
            alreadyExists.IsExclusionTag = true;
            alreadyExists.ExclusionKey = exclusionKey;
            alreadyExists.UpdatedAt = DateTime.UtcNow;

            var reactivated = await UpdateAsync(alreadyExists, companyId);
            return ToCatalogItem(reactivated);
        }

        var created = await AddAsync(new Ingredient
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow,
            Name = dto.Name.Trim(),
            NormalizedName = normalizedName,
            IsAllergen = dto.IsAllergen,
            IsExclusionTag = true,
            ExclusionKey = exclusionKey
        }, companyId);

        return ToCatalogItem(created);
    }

    public async Task RemoveCatalogItemAsync(Guid companyId, Guid ingredientId)
    {
        var existing = await GetByIdAsync(ingredientId, companyId);
        if (existing == null || existing.DeletedAt != null)
        {
            throw new KeyNotFoundException($"Ingredient {ingredientId} was not found in company scope {companyId}.");
        }

        existing.DeletedAt = DateTime.UtcNow;
        existing.UpdatedAt = DateTime.UtcNow;
        await UpdateAsync(existing, companyId);
    }

    private static IngredientCatalogItemDto ToCatalogItem(Ingredient ingredient)
    {
        return new IngredientCatalogItemDto
        {
            IngredientId = ingredient.Id,
            Name = ingredient.Name,
            NormalizedName = ingredient.NormalizedName ?? NormalizeName(ingredient.Name),
            IsAllergen = ingredient.IsAllergen,
            IsExclusionTag = ingredient.IsExclusionTag,
            ExclusionKey = ingredient.ExclusionKey
        };
    }

    private static string NormalizeName(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}

