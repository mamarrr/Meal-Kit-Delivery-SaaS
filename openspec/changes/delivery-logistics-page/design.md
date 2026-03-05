## Context

The platform already models delivery-related data (zones, windows, deliveries, attempts, complaints) but company users lack a dedicated operations page that assembles these workflows in one place. The new Delivery Logistics page must serve company roles in a tenant-isolated context, align with existing MVC layering (WebApp → BLL → DAL), and reuse current delivery domain abstractions without introducing cross-tenant data exposure.

Constraints include existing role boundaries, path-based tenant routing, no-tracking EF query behavior that requires explicit `Update()` for modifications, and current UI architecture patterns that require strongly typed ViewModels (no `ViewBag`/`ViewData`).

Stakeholders: CompanyAdmin and CompanyManager (primary), CompanyEmployee (tracking-focused), and indirectly support/admin actors who rely on clean operational records.

## Goals / Non-Goals

**Goals:**
- Provide one company-facing Delivery Logistics page with clear sections for:
  - Delivery zones list
  - Zone schedules (days, windows, capacity limits)
  - Deliveries/runs and orders by date
  - Delivery tracking outcomes (success/failed, notes, proof metadata, reschedule)
  - Complaint-based escalation rules (freshest stock, earliest slot)
- Ensure all reads/writes remain tenant-scoped by `CompanyId`.
- Fit implementation into existing controller/service/repository boundaries with predictable authorization.
- Make the page implementation-ready through explicit integration points and rollout sequence.

**Non-Goals:**
- Building route optimization or map/GPS features.
- Replacing existing delivery domain entities with a new data model.
- Implementing real-time driver mobile tracking.
- Introducing new cross-tenant analytics for system-level roles in this change.

## Decisions

1. **Single-page operations surface with sectioned composition**
   - Decision: Implement one Delivery Logistics page with section-level partials/components instead of separate route-first subpages.
   - Rationale: Matches user preference, reduces navigation overhead for daily operations, and keeps context switching low.
   - Alternatives considered:
     - Separate subpages per concern (zones/schedules/runs/tracking/escalation): clearer URL boundaries but slower operator flow.
     - Dashboard-only summary plus drill-ins: less immediate control for operational updates.

2. **Controller orchestration with dedicated delivery ViewModels per section**
   - Decision: Add a delivery logistics controller/action pair that hydrates a composite ViewModel from existing BLL services; section-level POST actions handle updates.
   - Rationale: Preserves current MVC conventions and keeps business rules in BLL.
   - Alternatives considered:
     - Direct repository calls from controllers: violates layering and increases coupling.
     - API-first SPA: larger architectural shift than required.

3. **Reuse and extend existing delivery services/repositories before adding new services**
   - Decision: Extend current delivery contracts where necessary (e.g., date-filtered run/order projections, escalation rule retrieval/update) rather than parallel service trees.
   - Rationale: Minimizes duplication and keeps behavior centralized.
   - Alternatives considered:
     - New isolated “logistics facade” only in WebApp: fast short-term but duplicates business logic.

4. **Escalation represented as deterministic priority policy inputs**
   - Decision: Store/handle escalation rules as explicit policy settings tied to complaints context (e.g., prioritize freshest stock first, earliest slot fallback).
   - Rationale: Enables auditable and testable behavior versus implicit/manual heuristics.
   - Alternatives considered:
     - Free-form notes-driven escalation only: insufficiently deterministic and hard to validate.

5. **No-tracking-safe update pathways**
   - Decision: Any edit flows on schedules/tracking/escalation require BLL paths that call repository update semantics compatible with no-tracking behavior.
   - Rationale: Prevents silent update failures under `NoTrackingWithIdentityResolution`.
   - Alternatives considered:
     - Relying on implicit tracked entities: incompatible with current DbContext behavior.

## Risks / Trade-offs

- **[Risk]** Single-page scope could become dense and harder to maintain. → **Mitigation:** modular section view models/partials and thin controller methods.
- **[Risk]** Mixed read/write concerns on one page increase accidental authorization gaps. → **Mitigation:** explicit per-action role attributes and test coverage by role.
- **[Risk]** Delivery and complaint data joins may become expensive for date-based lists. → **Mitigation:** projection-focused repository queries and pagination/date window defaults.
- **[Risk]** Escalation policy semantics could be ambiguous across teams. → **Mitigation:** codify enum-like policy options and display plain-language help text in UI.

## Migration Plan

1. Add/extend BLL and DAL contracts for missing read/write operations (schedules capacity, run/order projections, escalation policy persistence).
2. Add controller, composite ViewModel, and Razor page sections for all required logistics concerns.
3. Wire company navigation entry and route mapping under existing company operations path conventions.
4. Add/adjust authorization attributes and tenant filtering checks.
5. Validate with targeted unit/integration tests for tenant isolation, role permissions, and update persistence.
6. Rollout with feature parity checks against existing operational flows.

Rollback strategy: remove navigation entry and route exposure first; retain data schema additions as backward-compatible where possible, and disable writes if partial rollback is required.

## Open Questions

- Should proof capture store only metadata/reference (e.g., URL/token) in this phase, or include direct upload handling?
- Should escalation rules be global per company or overridable per zone/time window?
- What default date range should be used for runs/orders list for acceptable performance and usability?
- Are free-tier limits (e.g., single zone) enforced directly in this page interactions or through shared policy middleware only?
