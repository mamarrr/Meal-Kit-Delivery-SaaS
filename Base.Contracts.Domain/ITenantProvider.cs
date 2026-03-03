namespace Base.Contracts.Domain;

/// <summary>
/// Marker interface for entities that belong to a specific company tenant.
/// Implementing this interface indicates the entity stores business data
/// that must be isolated per tenant using the CompanyId discriminator.
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// The identifier of the company tenant that owns this entity.
    /// </summary>
    Guid CompanyId { get; set; }
}
