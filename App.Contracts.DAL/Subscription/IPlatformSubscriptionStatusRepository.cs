using App.Domain.Subscription;
using Contracts.DAL;

namespace App.Contracts.DAL.Subscription;

/// <summary>
/// Repository interface for PlatformSubscriptionStatus lookup aggregate.
/// </summary>
public interface IPlatformSubscriptionStatusRepository : IRepository<PlatformSubscriptionStatus>
{
}

