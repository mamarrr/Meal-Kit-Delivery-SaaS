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

    public async Task<ICollection<DietaryCategoryCatalogItemDto>> GetCatalogAsync(Guid companyId)
    {
        var rows = await Repository.GetAllByCompanyIdAsync(companyId);

        return rows
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.Name)
            .Select(x => new DietaryCategoryCatalogItemDto
            {
                DietaryCategoryId = x.Id,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToList();
    }

    public async Task<DietaryCategoryCatalogItemDto> UpsertCatalogItemAsync(Guid companyId, Guid actorId, DietaryCategoryCatalogUpsertDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentException("Dietary category name is required.", nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(dto.Code))
        {
            throw new ArgumentException("Dietary category code is required.", nameof(dto));
        }

        var normalizedCode = dto.Code.Trim().ToLowerInvariant();
        var all = await Repository.GetAllByCompanyIdAsync(companyId);

        if (dto.DietaryCategoryId.HasValue)
        {
            var existing = await GetByIdAsync(dto.DietaryCategoryId.Value, companyId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Dietary category {dto.DietaryCategoryId.Value} was not found in company scope {companyId}.");
            }

            var duplicate = all.FirstOrDefault(x =>
                x.DeletedAt == null
                && x.Id != existing.Id
                && string.Equals(x.Code, normalizedCode, StringComparison.OrdinalIgnoreCase));

            if (duplicate != null)
            {
                throw new InvalidOperationException("Dietary category code must be unique within company scope.");
            }

            existing.Name = dto.Name.Trim();
            existing.Code = normalizedCode;
            existing.IsActive = dto.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await UpdateAsync(existing, companyId);
            return ToCatalogItem(updated);
        }

        var codeExists = all.Any(x =>
            x.DeletedAt == null
            && string.Equals(x.Code, normalizedCode, StringComparison.OrdinalIgnoreCase));

        if (codeExists)
        {
            throw new InvalidOperationException("Dietary category code must be unique within company scope.");
        }

        var created = await AddAsync(new DietaryCategory
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow,
            Code = normalizedCode,
            Name = dto.Name.Trim(),
            IsActive = dto.IsActive
        }, companyId);

        return ToCatalogItem(created);
    }

    private static DietaryCategoryCatalogItemDto ToCatalogItem(DietaryCategory category)
    {
        return new DietaryCategoryCatalogItemDto
        {
            DietaryCategoryId = category.Id,
            Code = category.Code,
            Name = category.Name,
            IsActive = category.IsActive
        };
    }
}

