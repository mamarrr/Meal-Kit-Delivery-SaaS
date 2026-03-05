using App.Contracts.DAL.Core;
using App.Domain.Core;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Core;

/// <summary>
/// EF Core repository implementation for Customer aggregate.
/// </summary>
public class CustomerRepository : BaseRepository<Customer, AppDbContext>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<Customer>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(c => c.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<ICollection<CustomerSubscriberListItem>> GetSubscriberListAsync(Guid companyId, string? search, string? status, string? tier, Guid? deliveryZoneId)
    {
        var normalizedSearch = search?.Trim().ToLowerInvariant();
        var normalizedStatus = status?.Trim().ToLowerInvariant();
        var normalizedTier = tier?.Trim().ToLowerInvariant();

        var query = RepositoryDbSet
            .Where(c => c.CompanyId == companyId)
            .Select(c => new
            {
                Customer = c,
                ActiveSubscription = RepositoryDbContext.MealSubscriptions
                    .Include(ms => ms.Box)
                    .Where(ms => ms.CompanyId == companyId && ms.CustomerId == c.Id)
                    .OrderByDescending(ms => ms.IsActive)
                    .ThenByDescending(ms => ms.StartDate)
                    .FirstOrDefault(),
                LatestDelivery = RepositoryDbContext.Deliveries
                    .Include(d => d.DeliveryZone)
                    .Where(d => d.CompanyId == companyId && d.CustomerId == c.Id)
                    .OrderByDescending(d => d.ScheduledTime)
                    .FirstOrDefault()
            });

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(x =>
                x.Customer.FirstName.ToLower().Contains(normalizedSearch)
                || x.Customer.LastName.ToLower().Contains(normalizedSearch)
                || x.Customer.Email.ToLower().Contains(normalizedSearch));
        }

        if (!string.IsNullOrWhiteSpace(normalizedStatus))
        {
            query = query.Where(x =>
                normalizedStatus == "active"
                    ? x.ActiveSubscription != null && x.ActiveSubscription.IsActive
                    : x.ActiveSubscription == null || !x.ActiveSubscription.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(normalizedTier))
        {
            query = query.Where(x => x.ActiveSubscription != null
                && x.ActiveSubscription.Box != null
                && x.ActiveSubscription.Box.DisplayName.ToLower().Contains(normalizedTier));
        }

        if (deliveryZoneId.HasValue)
        {
            query = query.Where(x => x.LatestDelivery != null && x.LatestDelivery.DeliveryZoneId == deliveryZoneId.Value);
        }

        var data = await query
            .OrderBy(x => x.Customer.FirstName)
            .ThenBy(x => x.Customer.LastName)
            .Select(x => new CustomerSubscriberListItem
            {
                CustomerId = x.Customer.Id,
                FullName = (x.Customer.FirstName + " " + x.Customer.LastName).Trim(),
                Email = x.Customer.Email,
                Status = x.ActiveSubscription != null && x.ActiveSubscription.IsActive ? "Active" : "Inactive",
                Tier = x.ActiveSubscription != null && x.ActiveSubscription.Box != null
                    ? x.ActiveSubscription.Box.DisplayName
                    : "Unassigned",
                DeliveryZoneId = x.LatestDelivery != null ? x.LatestDelivery.DeliveryZoneId : null,
                DeliveryZoneName = x.LatestDelivery != null && x.LatestDelivery.DeliveryZone != null
                    ? x.LatestDelivery.DeliveryZone.Name
                    : string.Empty
            })
            .ToListAsync();

        return data;
    }

    public async Task<CustomerSubscriberDetails?> GetSubscriberDetailsAsync(Guid companyId, Guid customerId)
    {
        var customer = await RepositoryDbSet
            .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.Id == customerId);

        if (customer == null)
        {
            return null;
        }

        var latestSubscription = await RepositoryDbContext.MealSubscriptions
            .Include(ms => ms.Box)
            .Where(ms => ms.CompanyId == companyId && ms.CustomerId == customerId)
            .OrderByDescending(ms => ms.IsActive)
            .ThenByDescending(ms => ms.StartDate)
            .FirstOrDefaultAsync();

        var latestDeliveryZone = await RepositoryDbContext.Deliveries
            .Include(d => d.DeliveryZone)
            .Where(d => d.CompanyId == companyId && d.CustomerId == customerId)
            .OrderByDescending(d => d.ScheduledTime)
            .Select(d => d.DeliveryZone)
            .FirstOrDefaultAsync();

        var preferences = await RepositoryDbContext.CustomerPreferences
            .Include(cp => cp.DietaryCategory)
            .Where(cp => cp.CustomerId == customerId && cp.Customer!.CompanyId == companyId && cp.DeletedAt == null)
            .OrderBy(cp => cp.DietaryCategory!.Name)
            .Select(cp => cp.DietaryCategory!.Name)
            .ToListAsync();

        var exclusions = await RepositoryDbContext.CustomerExclusions
            .Include(ce => ce.Ingredient)
            .Where(ce => ce.CustomerId == customerId && ce.Customer!.CompanyId == companyId && ce.DeletedAt == null)
            .OrderBy(ce => ce.Ingredient!.Name)
            .Select(ce => ce.Ingredient!.Name)
            .ToListAsync();

        var ratings = await RepositoryDbContext.Ratings
            .Include(r => r.Recipe)
            .Where(r => r.CustomerId == customerId && r.Customer!.CompanyId == companyId && r.DeletedAt == null)
            .OrderByDescending(r => r.RatedAt)
            .Select(r => new SubscriberRatingHistoryItem
            {
                RatedAt = r.RatedAt,
                RecipeName = r.Recipe != null ? r.Recipe.Name : string.Empty,
                Score = r.Score,
                Notes = r.Notes
            })
            .ToListAsync();

        var lifecycle = await RepositoryDbContext.MealSubscriptions
            .Where(ms => ms.CompanyId == companyId && ms.CustomerId == customerId)
            .OrderByDescending(ms => ms.UpdatedAt ?? ms.StartDate)
            .Select(ms => new SubscriberLifecycleHistoryItem
            {
                OccurredAt = ms.UpdatedAt ?? ms.StartDate,
                EventType = ms.IsActive ? "Pause/Skip" : "Cancellation",
                Status = ms.IsActive ? "Active" : "Inactive",
                Details = ms.EndDate.HasValue
                    ? $"Effective until {ms.EndDate.Value:yyyy-MM-dd}"
                    : "No end date"
            })
            .ToListAsync();

        return new CustomerSubscriberDetails
        {
            CustomerId = customer.Id,
            FullName = (customer.FirstName + " " + customer.LastName).Trim(),
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            AddressLine = customer.AddressLine,
            City = customer.City,
            PostalCode = customer.PostalCode,
            Country = customer.Country,
            DeliveryZoneId = latestDeliveryZone?.Id,
            DeliveryZoneName = latestDeliveryZone?.Name ?? string.Empty,
            PlanBox = latestSubscription?.Box?.DisplayName ?? "Unassigned",
            Preferences = preferences,
            Exclusions = exclusions,
            RatingsHistory = ratings,
            LifecycleHistory = lifecycle
        };
    }
}
