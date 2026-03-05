using App.Contracts.BLL.Subscription;
using App.Contracts.DAL.Subscription;
using App.Domain.Subscription;

namespace App.BLL.Subscription;

public class BoxService : BaseTenantService<Box, IBoxRepository>, IBoxService
{
    public BoxService(IBoxRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<Box>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<int> CountActiveByCompanyIdAsync(Guid companyId)
    {
        return await Repository.CountActiveByCompanyIdAsync(companyId);
    }

    public async Task<ICollection<CustomerDiscoverableBoxDto>> GetDiscoverableBoxesAsync(CustomerBoxDiscoveryFilterDto filter)
    {
        var boxes = await Repository.GetDiscoverableBoxesAsync(
            filter.CompanyIds.Count > 0 ? filter.CompanyIds : null,
            filter.MinPrice,
            filter.MaxPrice,
            filter.DietaryCategoryIds.Count > 0 ? filter.DietaryCategoryIds : null);

        var now = DateTime.UtcNow;

        return boxes
            .Select(box =>
            {
                var activePrice = box.BoxPrices?
                    .Where(bp => bp.DeletedAt == null
                                 && bp.IsActive
                                 && (bp.ValidFrom == null || bp.ValidFrom <= now)
                                 && (bp.ValidTo == null || bp.ValidTo >= now))
                    .OrderBy(bp => bp.PriceAmount)
                    .Select(bp => (decimal?)bp.PriceAmount)
                    .FirstOrDefault();

                var dietaryIds = box.BoxDietaryCategories?
                    .Where(dc => dc.DeletedAt == null)
                    .Select(dc => dc.DietaryCategoryId)
                    .Distinct()
                    .ToArray() ?? [];

                return new CustomerDiscoverableBoxDto
                {
                    BoxId = box.Id,
                    CompanyId = box.CompanyId,
                    CompanyName = box.Company?.Name ?? string.Empty,
                    MealsCount = box.MealsCount,
                    PeopleCount = box.PeopleCount,
                    DisplayName = box.DisplayName,
                    ActivePrice = activePrice,
                    DietaryCategoryIds = dietaryIds
                };
            })
            .OrderBy(x => x.CompanyName)
            .ThenBy(x => x.DisplayName)
            .ToArray();
    }
}

