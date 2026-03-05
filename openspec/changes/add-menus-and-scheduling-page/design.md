## Context

The company side of the product currently has menu- and recipe-related entities/services, but no dedicated workflow-centered planning experience for weekly menus. Users need one coherent place to define weekly scheduling rules, assign recipes into week slots, and preview auto-selection behavior before subscribers are auto-assigned meals.

This change spans UI, service orchestration, and data access concerns in tenant-scoped company operations. It must respect existing multi-tenant constraints, role-based access boundaries, and EF Core no-tracking update behavior.

## Goals / Non-Goals

**Goals:**
- Introduce a dedicated Menus & Scheduling area in company view with navigation entry and role-appropriate access.
- Support week-centric planning with an explicit weekly view and recipe assignment flow.
- Support menu configuration (recipes per category, deadline settings, no-repeat N weeks).
- Provide an auto-selection simulation/preview path that demonstrates rule outcomes without publishing side effects.
- Reuse existing layered architecture and repository/service contracts, extending them where needed rather than bypassing layers.

**Non-Goals:**
- Building a new customer-facing selection UX in this change.
- Implementing cross-tenant/global scheduling behavior.
- Replacing existing recipe lifecycle and CRUD flows outside scheduling context.
- Introducing external scheduling engines or third-party optimization dependencies.

## Decisions

1. **Add a company-focused MVC module (controller + viewmodels + views) for Menus & Scheduling**
   - **Why:** The requirement is primarily workflow and operational UI behavior for company users.
   - **Alternative considered:** Extend existing recipe or weekly menu pages directly. Rejected due to poor discoverability and fragmented user flow.

2. **Represent scheduling around explicit week identity (ISO week/date range) in service API contracts**
   - **Why:** Calendar/week view and assignment rules are naturally week-bounded; explicit week keys reduce ambiguity in queries and validations.
   - **Alternative considered:** Implicit latest-week operations. Rejected because it breaks preview/review across historical/future weeks.

3. **Implement configuration/rule enforcement in BLL services, not controllers**
   - **Why:** Rule checks (category quotas, deadline constraints, no-repeat window) must be reusable and testable, and should not be duplicated across endpoints.
   - **Alternative considered:** Controller-level validation only. Rejected for maintainability and consistency reasons.

4. **Persist and update scheduling/config entities through repository interfaces with explicit update calls**
   - **Why:** EF is configured with `NoTrackingWithIdentityResolution`; updates must call repository/update methods explicitly to persist changes.
   - **Alternative considered:** Relying on tracked entity mutation. Rejected because changes may silently fail.

5. **Simulation is modeled as a dry-run service operation returning candidate selections and constraint diagnostics**
   - **Why:** Gives high value for planning accuracy while avoiding side effects in production data.
   - **Alternative considered:** Full auto-publish implementation immediately. Rejected to reduce rollout risk and preserve operator control.

6. **Authorization follows existing company operational role patterns**
   - **Why:** Menus & Scheduling is operational data; managers/admins need full access, while employee access can be constrained by policy.
   - **Alternative considered:** New custom role set. Rejected as unnecessary complexity for initial scope.

## Risks / Trade-offs

- **[Risk] Rule complexity may produce confusing validation conflicts for users** → Mitigation: return structured validation messages (quota, repeat-window, deadline) and expose them in the scheduling UI.
- **[Risk] Data model extensions for weekly configuration could increase migration complexity** → Mitigation: additive schema changes only, with backwards-compatible defaults (e.g., no-repeat default = 8).
- **[Risk] Simulation logic drift from real auto-selection logic** → Mitigation: both paths share the same core rule-evaluation service.
- **[Risk] Performance degradation on week view with large recipe pools** → Mitigation: tenant-scoped indexed queries, bounded date-range filtering, and projection-focused repository methods.
- **[Trade-off] Delivering simulator first (without full auto-publish orchestration) delays full automation** → Benefit: lower risk rollout and earlier planning feedback for operators.

## Migration Plan

1. Add/extend domain and persistence model for weekly configuration and assignment metadata (tenant-scoped, soft-delete compatible where applicable).
2. Add DAL/BLL contract changes and implementations for week planning queries, assignment operations, and rule simulation.
3. Add company controller/viewmodels/views and navigation entry for Menus & Scheduling.
4. Add authorization policy usage and role checks aligned with existing company operational access.
5. Validate with automated tests for rule enforcement and repository/service behavior.
6. Rollout with feature-complete UI route in company workspace; rollback by disabling route/navigation and reverting migration if needed.

## Open Questions

- Should CompanyEmployee have read-only or limited write access to recipe assignment in this first release?
- Do deadline rules need per-delivery-zone override now, or only company-level/global settings for the weekly menu?
- Should simulation output include deterministic tie-break explanations to support auditing of recipe choices?
