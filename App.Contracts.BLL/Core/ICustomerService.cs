using App.Domain.Core;

namespace App.Contracts.BLL.Core;

public interface ICustomerService : ITenantEntityService<Customer>
{
    Task<ICollection<SubscriberListItem>> GetSubscriberListAsync(Guid companyId, string? search, string? status, string? tier, Guid? deliveryZoneId);
    Task<SubscriberDetails?> GetSubscriberDetailsAsync(Guid companyId, Guid customerId);
}

public sealed class SubscriberListItem
{
    public Guid CustomerId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Tier { get; init; } = string.Empty;
    public Guid? DeliveryZoneId { get; init; }
    public string DeliveryZoneName { get; init; } = string.Empty;
}

public sealed class SubscriberDetails
{
    public Guid CustomerId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }

    public string AddressLine { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;

    public Guid? DeliveryZoneId { get; init; }
    public string DeliveryZoneName { get; init; } = string.Empty;
    public string PlanBox { get; init; } = string.Empty;

    public ICollection<string> Preferences { get; init; } = [];
    public ICollection<string> Exclusions { get; init; } = [];
    public ICollection<SubscriberRatingHistoryItem> RatingsHistory { get; init; } = [];
    public ICollection<SubscriberLifecycleHistoryItem> LifecycleHistory { get; init; } = [];
}

public sealed class SubscriberRatingHistoryItem
{
    public DateTime RatedAt { get; init; }
    public string RecipeName { get; init; } = string.Empty;
    public int Score { get; init; }
    public string Notes { get; init; } = string.Empty;
}

public sealed class SubscriberLifecycleHistoryItem
{
    public DateTime OccurredAt { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Details { get; init; } = string.Empty;
}

