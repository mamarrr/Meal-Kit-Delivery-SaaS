## Context

The system is a multi-tenant SaaS where tenant isolation depends on filtering business data by `CompanyId`. Current domain entities model tenant-owned data but do not expose a single contract that data-access layers can rely on to identify tenant-scoped entities consistently. This creates a cross-cutting risk: different repositories or query paths may apply tenant filters inconsistently, increasing the chance of cross-tenant data exposure.

This change is intentionally limited to a foundational contract and its application across domain entities. It aligns with existing architecture constraints in this codebase (layered architecture, shared base entities, EF Core no-tracking query behavior) and prepares for safer repository/query composition.

## Goals / Non-Goals

**Goals:**
- Introduce a single domain contract (`ITenantProvider`) that identifies tenant-scoped entities via `CompanyId`.
- Apply the contract to all domain entities that must be company-isolated.
- Make tenant filtering requirements explicit so repositories/services can enforce `CompanyId` constraints uniformly.
- Keep the change backwards-compatible for API surfaces and database schema where possible.

**Non-Goals:**
- Re-architecting all repositories into a new generic tenant repository hierarchy.
- Implementing path-based tenant resolution or authorization redesign.
- Adding soft-delete/audit features in this change.
- Changing non-tenant/global entities to carry `CompanyId` where it is semantically invalid.

## Decisions

1. Define a dedicated interface in the domain layer.
- Decision: Add `ITenantProvider` in the domain (or shared domain-contract location used by domain entities) with `Guid CompanyId`.
- Rationale: Domain-level contract keeps tenant-scoping close to business entities and avoids DAL-only assumptions.
- Alternative considered: Convention-based reflection (detect `CompanyId` property by name).
  - Rejected because conventions are implicit, fragile, and harder to validate at compile time.

2. Use explicit implementation on tenant-owned entities.
- Decision: Each company-scoped entity explicitly implements `ITenantProvider`.
- Rationale: Strong compile-time signal for developers and future tooling; easier to audit which entities require tenant filtering.
- Alternative considered: Inherit from a separate tenant base entity.
  - Rejected for now to avoid hierarchy churn and broad refactoring risk across an existing domain model.

3. Keep filtering enforcement in repository/query boundaries, driven by the contract.
- Decision: Repository/query logic that accesses tenant data must enforce `CompanyId == currentTenantId` for entities implementing `ITenantProvider`.
- Rationale: Preserves separation of concerns (domain defines scope; DAL enforces scope).
- Alternative considered: EF global query filters for all tenant entities.
  - Deferred because it requires wider `DbContext` design decisions and tenant context injection strategy beyond this change scope.

4. Preserve non-tenant/global entity semantics.
- Decision: Do not force `ITenantProvider` on platform/global entities.
- Rationale: Some entities are intentionally cross-tenant/system-scoped; incorrectly tagging them would create artificial constraints and potential data modeling issues.
- Alternative considered: Universal tenant interface with nullable `CompanyId`.
  - Rejected because nullable contracts weaken guarantees and complicate enforcement logic.

## Risks / Trade-offs

- [Risk] Missed entity coverage leaves isolation gaps → Mitigation: Enumerate and verify all company-scoped entities during implementation and require review checklist validation.
- [Risk] Developers may bypass repository filters in ad-hoc queries → Mitigation: Add explicit spec requirements and implementation tasks for query enforcement patterns.
- [Risk] Ambiguity around entities related to multiple companies/users → Mitigation: Classify entity ownership explicitly in specs and exclude non-owned join/system entities from contract unless truly tenant-owned.
- [Trade-off] Interface-only approach is lighter but does not enforce behavior by itself → Mitigation: Pair contract with mandatory repository/query requirements in specs and tasks.

## Migration Plan

1. Add `ITenantProvider` contract.
2. Update tenant-owned entities to implement it.
3. Update data-access/query paths for those entities to apply `CompanyId` filtering from tenant context.
4. Validate behavior with targeted tests or verification steps (tenant A cannot read tenant B data).
5. Rollback strategy: remove interface implementation changes and tenant-filter enforcement changes together if regressions occur; no irreversible schema migration is expected from this change.

## Open Questions

- Should tenant filtering be enforced purely in repositories, or partially in `DbContext` via global query filters in a follow-up?
- What is the canonical source for current tenant context in all DAL execution paths (HTTP request scope, service context, or both)?
- Are there any edge entities shared across tenant boundaries that require explicit exemption documentation?