## Why

Multi-tenant isolation in this platform requires every company-scoped entity to be consistently identifiable so data access can always enforce `CompanyId` filtering. Adding a dedicated tenant-scoping interface now closes a critical gap before additional data-access features increase the risk of cross-tenant leakage.

## What Changes

- Introduce a domain-level marker contract named `ITenantProvider` that exposes `CompanyId` for tenant-scoped entities.
- Require all company-owned domain entities (where data must be isolated per tenant) to implement `ITenantProvider`.
- Establish requirement-level behavior that tenant-scoped queries and repository operations must filter by the current tenant’s `CompanyId`.
- Clarify that global/system entities that are not tenant-owned must not implement this contract.
- No external API contract changes are introduced in this change.

## Capabilities

### New Capabilities
- `tenant-provider-contract`: Define and enforce a shared tenant-scoping interface for company-owned entities to support safe multi-tenant filtering.

### Modified Capabilities
- `tenant-scoped-relationships`: Strengthen requirements so tenant-scoped entity access is consistently based on an explicit interface-driven `CompanyId` contract.

## Impact

- Affected code: Domain entity model in [`App.Domain/`](App.Domain/), tenant filtering logic in repositories/context usage in [`App.DAL.EF/`](App.DAL.EF/).
- Affected specs: New delta spec for `tenant-provider-contract`, and updates to existing [`openspec/specs/tenant-scoped-relationships/spec.md`](openspec/specs/tenant-scoped-relationships/spec.md).
- Dependencies/systems: No new runtime dependencies; impacts multi-tenant data-isolation behavior and developer conventions for future entities.
