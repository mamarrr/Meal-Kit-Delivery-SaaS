using App.Contracts.BLL.Subscription;
using App.Contracts.DAL.Menu;
using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;

namespace App.BLL.Subscription;

public class PricingProductsService(
    IBoxRepository boxRepository,
    IBoxPriceRepository boxPriceRepository,
    IBoxDietaryCategoryRepository boxDietaryCategoryRepository,
    IDietaryCategoryRepository dietaryCategoryRepository,
    IPricingAdjustmentRepository pricingAdjustmentRepository) : IPricingProductsService
{
    public async Task<PricingProductsPageDto> GetPageAsync(Guid companyId)
    {
        var boxes = (await boxRepository.GetAllByCompanyIdAsync(companyId))
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.MealsCount)
            .ThenBy(x => x.PeopleCount)
            .ToList();

        var prices = (await boxPriceRepository.GetAllByCompanyIdWithDetailsAsync(companyId))
            .Where(x => x.DeletedAt == null)
            .ToList();

        var boxCategoryLinks = (await boxDietaryCategoryRepository.GetAllByCompanyIdAsync(companyId))
            .Where(x => x.DeletedAt == null)
            .ToList();

        var dietaryCategories = (await dietaryCategoryRepository.GetAllByCompanyIdAsync(companyId))
            .Where(x => x.DeletedAt == null && x.IsActive)
            .OrderBy(x => x.Name)
            .ToList();

        var adjustments = (await pricingAdjustmentRepository.GetAllByCompanyIdAsync(companyId))
            .Where(x => x.DeletedAt == null)
            .ToList();

        var pricingConfigurations = prices
            .Select(x => x.PricingName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Select(pricingName => new PricingConfigurationDto
            {
                PricingName = pricingName,
                BoxPrices = boxes
                    .Select(box =>
                    {
                        var boxPrice = prices.FirstOrDefault(x => x.BoxId == box.Id
                                                                  && string.Equals(x.PricingName, pricingName, StringComparison.OrdinalIgnoreCase));
                        return new PricingBoxPriceRowDto
                        {
                            BoxPriceId = boxPrice?.Id,
                            BoxId = box.Id,
                            BoxLabel = box.DisplayName,
                            PriceAmount = boxPrice?.PriceAmount,
                            IsActive = boxPrice?.IsActive ?? false
                        };
                    })
                    .ToList()
            })
            .ToList();

        return new PricingProductsPageDto
        {
            Boxes = boxes
                .Select(x => new PricingBoxListItemDto
                {
                    BoxId = x.Id,
                    MealsCount = x.MealsCount,
                    PeopleCount = x.PeopleCount,
                    Name = x.DisplayName,
                    IsActive = x.IsActive,
                    AllowedDietaryCategoryIds = boxCategoryLinks
                        .Where(link => link.BoxId == x.Id)
                        .Select(link => link.DietaryCategoryId)
                        .Distinct()
                        .ToList()
                })
                .ToList(),
            PricingConfigurations = pricingConfigurations,
            DietaryCategories = dietaryCategories
                .Select(x => new PricingDietaryCategoryOptionDto
                {
                    DietaryCategoryId = x.Id,
                    Name = x.Name
                })
                .ToList(),
            DeliveryFees = adjustments
                .Where(x => string.Equals(x.AdjustmentType, PricingConstants.DeliveryFeeType, StringComparison.OrdinalIgnoreCase))
                .Select(MapAdjustment)
                .ToList(),
            Discounts = adjustments
                .Where(x => string.Equals(x.AdjustmentType, PricingConstants.DiscountType, StringComparison.OrdinalIgnoreCase))
                .Select(MapAdjustment)
                .ToList()
        };
    }

    public async Task UpsertBoxAsync(Guid companyId, Guid actorId, PricingBoxUpsertDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentException("Box name is required.");
        }

        ValidateBox(dto.MealsCount, dto.PeopleCount);
        var allowedCategoryIds = dto.AllowedDietaryCategoryIds.Distinct().ToList();
        var allCategories = await dietaryCategoryRepository.GetAllByCompanyIdAsync(companyId);
        var categoryCount = allCategories
            .Count(x => x.DeletedAt == null && x.IsActive && allowedCategoryIds.Contains(x.Id));
        if (categoryCount != allowedCategoryIds.Count)
        {
            throw new ArgumentException("One or more selected dietary categories are invalid for the active company scope.");
        }

        var existingByCombo = (await boxRepository.GetAllByCompanyIdAsync(companyId))
            .FirstOrDefault(x => x.DeletedAt == null
                                 && x.MealsCount == dto.MealsCount
                                 && x.PeopleCount == dto.PeopleCount
                                 && (!dto.BoxId.HasValue || x.Id != dto.BoxId.Value));
        if (existingByCombo != null)
        {
            throw new ArgumentException("This box combination already exists for the company.");
        }

        if (dto.BoxId.HasValue)
        {
            var existing = await boxRepository.GetByIdAsync(dto.BoxId.Value);
            if (existing == null || existing.CompanyId != companyId || existing.DeletedAt != null)
            {
                throw new KeyNotFoundException($"Box {dto.BoxId.Value} was not found in company scope {companyId}.");
            }

            existing.MealsCount = dto.MealsCount;
            existing.PeopleCount = dto.PeopleCount;
            existing.DisplayName = dto.Name.Trim();
            existing.IsActive = dto.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            boxRepository.Update(existing);

            await boxDietaryCategoryRepository.RemoveAllByBoxIdAsync(existing.Id, companyId);
            foreach (var categoryId in allowedCategoryIds)
            {
                boxDietaryCategoryRepository.Add(new BoxDietaryCategory
                {
                    Id = Guid.NewGuid(),
                    BoxId = existing.Id,
                    DietaryCategoryId = categoryId,
                    CompanyId = companyId,
                    CreatedByAppUserId = actorId,
                    CreatedAt = DateTime.UtcNow
                });
            }
            return;
        }

        var newBoxId = Guid.NewGuid();
        boxRepository.Add(new Box
        {
            Id = newBoxId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow,
            MealsCount = dto.MealsCount,
            PeopleCount = dto.PeopleCount,
            DisplayName = dto.Name.Trim(),
            IsActive = dto.IsActive
        });

        foreach (var categoryId in allowedCategoryIds)
        {
            boxDietaryCategoryRepository.Add(new BoxDietaryCategory
            {
                Id = Guid.NewGuid(),
                BoxId = newBoxId,
                DietaryCategoryId = categoryId,
                CompanyId = companyId,
                CreatedByAppUserId = actorId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    public async Task UpsertPriceAsync(Guid companyId, Guid actorId, PricingBoxPriceUpsertDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.PricingName))
        {
            throw new ArgumentException("Pricing name is required.");
        }

        if (dto.PriceAmount < 0)
        {
            throw new ArgumentException("Price amount must be greater than or equal to 0.");
        }

        var box = await boxRepository.GetByIdAsync(dto.BoxId);
        if (box == null || box.CompanyId != companyId || box.DeletedAt != null)
        {
            throw new KeyNotFoundException("Selected box does not exist in tenant scope.");
        }

        var normalizedPricingName = dto.PricingName.Trim();

        if (dto.BoxPriceId.HasValue)
        {
            var existing = await boxPriceRepository.GetByIdAsync(dto.BoxPriceId.Value);
            if (existing == null || existing.CompanyId != companyId || existing.DeletedAt != null)
            {
                throw new KeyNotFoundException("Price row does not exist in tenant scope.");
            }

            existing.BoxId = dto.BoxId;
            existing.PricingName = normalizedPricingName;
            existing.PriceAmount = dto.PriceAmount;
            existing.IsActive = dto.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            boxPriceRepository.Update(existing);
            return;
        }

        var duplicate = await boxPriceRepository.GetByBoxAndPricingNameAsync(dto.BoxId, normalizedPricingName, companyId);
        if (duplicate != null && duplicate.DeletedAt == null)
        {
            duplicate.PriceAmount = dto.PriceAmount;
            duplicate.IsActive = dto.IsActive;
            duplicate.UpdatedAt = DateTime.UtcNow;
            boxPriceRepository.Update(duplicate);
            return;
        }

        boxPriceRepository.Add(new BoxPrice
        {
            Id = Guid.NewGuid(),
            BoxId = dto.BoxId,
            PricingName = normalizedPricingName,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow,
            PriceAmount = dto.PriceAmount,
            IsActive = dto.IsActive
        });
    }

    public async Task UpsertAdjustmentAsync(Guid companyId, Guid actorId, PricingAdjustmentUpsertDto dto)
    {
        ValidateAdjustmentType(dto.AdjustmentType);
        if (string.IsNullOrWhiteSpace(dto.Label))
        {
            throw new ArgumentException("Adjustment label is required.");
        }

        if (dto.Amount < 0)
        {
            throw new ArgumentException("Adjustment amount must be greater than or equal to 0.");
        }

        if (dto.IsPercentage && dto.Amount > 100)
        {
            throw new ArgumentException("Percentage adjustment cannot exceed 100.");
        }

        if (dto.AdjustmentId.HasValue)
        {
            var existing = await pricingAdjustmentRepository.GetByIdAsync(dto.AdjustmentId.Value);
            if (existing == null || existing.CompanyId != companyId || existing.DeletedAt != null)
            {
                throw new KeyNotFoundException("Pricing adjustment does not exist in tenant scope.");
            }

            existing.AdjustmentType = dto.AdjustmentType.Trim().ToLowerInvariant();
            existing.Label = dto.Label.Trim();
            existing.Amount = dto.Amount;
            existing.IsPercentage = dto.IsPercentage;
            existing.IsActive = dto.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            pricingAdjustmentRepository.Update(existing);
            return;
        }

        pricingAdjustmentRepository.Add(new PricingAdjustment
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow,
            AdjustmentType = dto.AdjustmentType.Trim().ToLowerInvariant(),
            Label = dto.Label.Trim(),
            Amount = dto.Amount,
            IsPercentage = dto.IsPercentage,
            IsActive = dto.IsActive
        });
    }

    public async Task DeactivateBoxAsync(Guid companyId, Guid boxId)
    {
        var box = await boxRepository.GetByIdAsync(boxId);
        if (box == null || box.CompanyId != companyId || box.DeletedAt != null)
        {
            throw new KeyNotFoundException("Box not found in tenant scope.");
        }

        box.IsActive = false;
        box.UpdatedAt = DateTime.UtcNow;
        box.DeletedAt ??= DateTime.UtcNow;
        boxRepository.Update(box);
    }

    public async Task DeactivateAdjustmentAsync(Guid companyId, Guid adjustmentId)
    {
        var adjustment = await pricingAdjustmentRepository.GetByIdAsync(adjustmentId);
        if (adjustment == null || adjustment.CompanyId != companyId || adjustment.DeletedAt != null)
        {
            throw new KeyNotFoundException("Pricing adjustment not found in tenant scope.");
        }

        adjustment.IsActive = false;
        adjustment.UpdatedAt = DateTime.UtcNow;
        adjustment.DeletedAt ??= DateTime.UtcNow;
        pricingAdjustmentRepository.Update(adjustment);
    }

    private static PricingAdjustmentListItemDto MapAdjustment(PricingAdjustment x)
    {
        return new PricingAdjustmentListItemDto
        {
            AdjustmentId = x.Id,
            AdjustmentType = x.AdjustmentType,
            Label = x.Label,
            Amount = x.Amount,
            IsPercentage = x.IsPercentage,
            IsActive = x.IsActive
        };
    }

    private static void ValidateBox(int meals, int people)
    {
        if (meals is not (3 or 4 or 5))
        {
            throw new ArgumentException("Meals per box must be one of: 3, 4, 5.");
        }

        if (people is not (2 or 4))
        {
            throw new ArgumentException("People per box must be one of: 2, 4.");
        }
    }

    private static void ValidateAdjustmentType(string adjustmentType)
    {
        var value = adjustmentType.Trim().ToLowerInvariant();
        if (!string.Equals(value, PricingConstants.DeliveryFeeType, StringComparison.Ordinal)
            && !string.Equals(value, PricingConstants.DiscountType, StringComparison.Ordinal))
        {
            throw new ArgumentException("Adjustment type must be either 'delivery_fee' or 'discount'.");
        }
    }
}
