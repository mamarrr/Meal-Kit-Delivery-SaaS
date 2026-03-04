## Context

The change introduces a BLL service layer between `WebApp` and the repository layer so business logic has a consistent abstraction and tenant isolation is enforced consistently. Repository interfaces and EF implementations already exist; service contracts and implementations do not. The solution follows a layered architecture (`WebApp → App.BLL → App.DAL.EF → App.Domain`) with strict tenant isolation (queries filtered by `CompanyId`), EF Core configured with `QueryTrackingBehavior.NoTrackingWithIdentityResolution`, and async CRUD patterns.

## Goals / Non-Goals

**Goals:**
- Define BLL service contracts for core, menu, delivery, subscription, and identity capabilities.
- Implement service classes that orchestrate repository calls, enforce tenant scoping, and apply business rules.
- Register services for DI so the WebApp consumes BLL contracts rather than repositories directly.
- Standardize service patterns (naming, base class conventions, async CRUD) across areas.

**Non-Goals:**
- Redesign existing repository interfaces or EF implementations.
- Introduce new external dependencies or change the database schema.
- Implement UI changes beyond updating DI and service usage.

## Decisions

- **Introduce a dedicated BLL contracts project/namespace (e.g., `App.Contracts.BLL`)** → Keeps service interfaces separate from implementations and aligns with the existing DAL contract pattern. Alternative: define contracts inside `App.BLL` only; rejected because it reduces layer clarity and limits testability.
- **Service interfaces expose async CRUD plus domain-specific operations per aggregate** → Matches repository patterns and ensures business-specific actions (like menu generation or delivery status updates) are centralized. Alternative: reuse repository interfaces directly in WebApp; rejected because it bypasses business rules and tenant safety.
- **Service implementations depend on repository interfaces and a tenant provider** → Enforces tenant isolation at service boundary and allows services to apply `CompanyId` filtering and validation. Alternative: rely solely on repository filters; rejected because it risks inconsistent enforcement and makes business rules harder to test.
- **Explicit update patterns for EF No-Tracking** → Service layer must call repository update methods that attach and mark entities as modified (or otherwise follow the required update pattern) before saving. Alternative: mutate entities directly after retrieval; rejected due to no-tracking behavior leading to silent no-op updates.
- **DI registrations in `App.BLL`** → Add extension methods to register all services in one place and call from `WebApp` startup. Alternative: register in `WebApp` directly; rejected to keep composition consistent and reusable for tests.

## Risks / Trade-offs

- **Risk:** Inconsistent tenant scoping across services → **Mitigation:** enforce a shared base service pattern that requires `CompanyId` and use tests to verify scoping for each service.
- **Risk:** Update operations silently fail due to No-Tracking → **Mitigation:** standardize repository update usage in services and document update flow in service templates.
- **Risk:** Service layer becomes a thin pass-through without business value → **Mitigation:** define clear service responsibilities (validation, orchestration, cross-repository logic) in specs and review during implementation.

## Migration Plan

- Add service contracts and implementations in `App.BLL` (and `App.Contracts.BLL` if used).
- Register services via DI extension method and wire in `WebApp` startup.
- Update controllers or application services to depend on BLL interfaces instead of repositories.
- Validate tenant scoping with existing integration tests (or add targeted tests).

## Open Questions

- Should a shared base service abstraction be introduced (e.g., `BaseService<TEntity, TRepo>`) to enforce patterns, or keep explicit implementations per service?
- Where should tenant scoping live if a user belongs to multiple companies — in the service or a dedicated tenant provider abstraction?