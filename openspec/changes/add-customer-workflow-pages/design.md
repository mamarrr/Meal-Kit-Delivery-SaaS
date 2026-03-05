## Context

The platform already contains foundational domain, repository, and service layers for subscriptions, menus, nutrition, deliveries, and complaints, but customer-facing workflow coverage is still incomplete for production use. This change must add a coherent customer journey spanning discovery, subscription lifecycle management, preferences/exclusions, deliveries, and complaints, while preserving strict tenant isolation and existing layered architecture constraints.

The solution must also formalize meal selection behavior under deadline pressure: customers select before cutoff, otherwise the system auto-selects from available recipes based on rating history and a configurable non-repetition window (default 8 weeks). Nutrition data and filtering must be exposed as customer-facing selection tools.

## Goals / Non-Goals

**Goals:**
- Deliver complete customer workflow pages for discover subscriptions, manage subscriptions, preferences/exclusions, deliveries, and complaints.
- Reuse and extend existing BLL/DAL capabilities with tenant-safe query boundaries and customer ownership checks.
- Define deterministic auto-selection behavior with configurable deadline and configurable repeat-blocking window.
- Expose nutritional filtering criteria (calories, protein, carbs, fat, fiber, sodium) in customer meal selection/discovery UX.
- Keep implementation aligned with existing MVC patterns (controller + ViewModel + Razor views), no ViewBag/ViewData.

**Non-Goals:**
- Redesigning company-side operations pages or roles.
- Introducing external recommendation engines or ML ranking.
- Reworking identity model or cross-tenant account linking.
- Full redesign of existing menu generation logic beyond the customer auto-selection path.

## Decisions

1. **Introduce dedicated customer workflow controllers and index/detail view models per page family**
   - **Decision:** Add/extend controllers and view models specifically for customer discovery, subscriptions, preferences, deliveries, and complaints.
   - **Why:** Keeps responsibilities explicit, fits current MVC structure, and supports page-specific filtering/pagination state.
   - **Alternative considered:** Single aggregated customer dashboard controller with mixed actions. Rejected due to coupling and reduced maintainability.

2. **Model customer discovery as subscription-product search over public boxes with tenant-scoped metadata joins**
   - **Decision:** Discovery queries aggregate public boxes with company, dietary tags, and active pricing into a filterable result.
   - **Why:** Aligns with required filters (company, price range, dietary categories) and supports subscribe action from the same workflow.
   - **Alternative considered:** Client-side filtering after broad load. Rejected due to performance and risk of leaking unnecessary tenant data.

3. **Auto-selection policy implemented as deterministic service-level rule pipeline**
   - **Decision:** Implement in BLL a policy sequence: (a) detect missed deadline, (b) build candidate recipes for relevant menu/week, (c) remove recent selections from configurable lookback window (default 8), (d) rank by customer ratings history and deterministic tiebreakers, (e) persist explicit selection records.
   - **Why:** Centralizes business invariants and ensures behavior is testable and auditable.
   - **Alternative considered:** Trigger/job-only implementation without service entry point. Rejected because user-triggered fallback and manual recomputation scenarios still need a callable policy surface.

4. **Nutrition filtering represented as explicit criteria object in BLL contract**
   - **Decision:** Use a typed nutrition filter DTO (min/max ranges per macro) passed through service/repository methods.
   - **Why:** Strong typing avoids query drift and supports extension without proliferating method overloads.
   - **Alternative considered:** Ad-hoc query-string parsing directly in repositories. Rejected due to duplication and weaker validation.

5. **Preferences/exclusions source list constrained to subscribed companies**
   - **Decision:** Customer preference and ingredient exclusion options are derived from currently subscribed companies only.
   - **Why:** Matches requirement, reduces irrelevant options, and respects tenant operational boundaries.
   - **Alternative considered:** Global master ingredient/category list. Rejected due to mismatch with per-company catalog variation.

6. **No-tracking EF update discipline enforced in all modifying paths**
   - **Decision:** Every update path explicitly calls `Update()` on entities loaded under `NoTrackingWithIdentityResolution` before save.
   - **Why:** Prevents silent write failures under current DbContext defaults.
   - **Alternative considered:** Enabling tracking for selected query paths. Rejected to avoid inconsistent behavior and hidden coupling.

## Risks / Trade-offs

- **[Risk] Cross-tenant data leakage in discovery aggregation** → **Mitigation:** enforce explicit company scoping and "public" visibility predicates in repository queries and add integration tests for unauthorized visibility.
- **[Risk] Auto-selection perceived as unfair or opaque** → **Mitigation:** deterministic ranking with explainable ordering fields and logged reason codes for selected meals.
- **[Risk] Non-repetition window causing no valid candidates** → **Mitigation:** controlled fallback strategy (progressive relaxation with explicit marker) and user-visible notification.
- **[Risk] Nutrition filters increase query complexity and latency** → **Mitigation:** indexed numeric columns, bounded filters, and paginated search.
- **[Trade-off] Separate page families increase surface area** → **Mitigation:** shared query/service helpers and common ViewModel conventions.

## Migration Plan

1. Add/extend BLL and DAL contracts for customer discovery filters, nutrition criteria, and auto-selection policy operations.
2. Implement repository query methods with tenant-safe predicates and required joins for discovery/preferences/deliveries/complaints.
3. Implement service-layer policy for deadline handling, non-repetition window, and rating-based ranking.
4. Add MVC controllers, view models, and Razor views for all customer workflow pages.
5. Add/adjust persistence schema only if required by missing fields (configurable lookback/deadline metadata or selection audit attributes), with EF migration.
6. Validate with unit/integration tests for filtering, authorization boundaries, and auto-selection behavior.
7. Rollout with feature toggle if needed; rollback by disabling customer workflow routes and reverting migration if schema changed.

## Open Questions

- Should configurable deadline and non-repetition settings be company-level, subscription-level, or box-level overrides (with inheritance)?
- When no candidates remain after policy constraints, should fallback prioritize least-recently-served or highest-rated repeated meals?
- Should complaints page include only delivered meals tied to active subscriptions, or full historical deliveries regardless of subscription status?
- Do nutritional filters apply only to meal selection flow, or also to subscription discovery cards where recipe previews are shown?
