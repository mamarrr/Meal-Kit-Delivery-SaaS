using App.Contracts.BLL.Core;
using App.Contracts.DAL.Core;
using App.Domain.Core;

namespace App.BLL.Core;

public class CustomerService : BaseTenantService<Customer, ICustomerRepository>, ICustomerService
{
    public CustomerService(ICustomerRepository repository) : base(repository)
    {
    }

    protected override async Task<ICollection<Customer>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<ICollection<SubscriberListItem>> GetSubscriberListAsync(Guid companyId, string? search, string? status, string? tier, Guid? deliveryZoneId)
    {
        var data = await Repository.GetSubscriberListAsync(companyId, search, status, tier, deliveryZoneId);
        return data.Select(x => new SubscriberListItem
        {
            CustomerId = x.CustomerId,
            FullName = x.FullName,
            Email = x.Email,
            Status = x.Status,
            Tier = x.Tier,
            DeliveryZoneId = x.DeliveryZoneId,
            DeliveryZoneName = x.DeliveryZoneName
        }).ToList();
    }

    public async Task<SubscriberDetails?> GetSubscriberDetailsAsync(Guid companyId, Guid customerId)
    {
        var data = await Repository.GetSubscriberDetailsAsync(companyId, customerId);
        if (data == null)
        {
            return null;
        }

        return new SubscriberDetails
        {
            CustomerId = data.CustomerId,
            FullName = data.FullName,
            Email = data.Email,
            PhoneNumber = data.PhoneNumber,
            AddressLine = data.AddressLine,
            City = data.City,
            PostalCode = data.PostalCode,
            Country = data.Country,
            DeliveryZoneId = data.DeliveryZoneId,
            DeliveryZoneName = data.DeliveryZoneName,
            PlanBox = data.PlanBox,
            Preferences = data.Preferences,
            Exclusions = data.Exclusions,
            RatingsHistory = data.RatingsHistory
                .Select(x => new App.Contracts.BLL.Core.SubscriberRatingHistoryItem
                {
                    RatedAt = x.RatedAt,
                    RecipeName = x.RecipeName,
                    Score = x.Score,
                    Notes = x.Notes
                }).ToList(),
            LifecycleHistory = data.LifecycleHistory
                .Select(x => new App.Contracts.BLL.Core.SubscriberLifecycleHistoryItem
                {
                    OccurredAt = x.OccurredAt,
                    EventType = x.EventType,
                    Status = x.Status,
                    Details = x.Details
                }).ToList()
        };
    }
}

