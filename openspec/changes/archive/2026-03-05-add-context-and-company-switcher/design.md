## Context

The authenticated shell currently exposes limited context switching and resolves company scope primarily from default claim enrichment. This does not support explicit user intent when a single `AppUser` has:
- multiple company memberships,
- mixed role surfaces across company and system domains,
- customer browsing needs independent from operational company actions.

The selected context and selected company must become first-class session inputs for route resolution and authorization behavior.

## Goals / Non-Goals

**Goals**
- Provide one topbar control area for choosing active context and active company.
- Support three contexts: `customer`, `company`, `system`.
- Restrict `system` context to `SystemAdmin`, `SystemSupport`, `SystemBilling`.
- Allow users with multiple active company memberships to choose active company explicitly.
- Make company choice URL-visible via `/{slug}/entry` routing.
- Preserve hybrid boundary: shared shell orchestration plus existing area/workspace boundaries.

**Non-Goals**
- Introducing full new `Customer` and `Company` Areas in this change.
- Rebuilding broader navigation IA beyond topbar selector integration.
- Replacing identity subsystem or role model.

## Decisions

1. **Shared-shell selector, role-specific workspaces**
   - Decision: Keep selector UI and selection endpoints in shared shell/routing layer.
   - Rationale: Centralizes cross-context mechanics and avoids duplicate controls per area.

2. **Two-part active state**
   - Decision: Persist both `active_context` and `active_company` as server-validated session cookies.
   - Rationale: Context and tenant are independent dimensions; both are needed for deterministic behavior.

3. **Slug-first company redirect**
   - Decision: Company selection redirects to `/{slug}/entry`.
   - Rationale: URL reflects tenant scope, improves shareability/debuggability, aligns with slug routing contract.

4. **Authorization-first selection validation**
   - Decision: Selection endpoints must validate requested context/company against authenticated user roles and memberships before persisting.
   - Rationale: Prevents forged requests and inconsistent state.

## Data / Lifecycle Flow

```text
Topbar submit context/company
        |
        v
Switch endpoint validates roles + memberships
        |
        +-- invalid -> fallback default + warning
        |
        v
Persist active_context + active_company
        |
        v
Redirect
  - company -> /{slug}/entry
  - customer/system -> /entry
        |
        v
Claims/context resolution applies active selection
```

## Risks / Trade-offs

- **Risk:** stale company cookie after membership removal.
  - **Mitigation:** validate cookie company on each transform/request path and clear invalid selection.
- **Risk:** role drift if every user is expected to have customer capability but seed/data reality differs.
  - **Mitigation:** treat customer context availability as explicit rule in endpoint validation and add migration/seed alignment task.
- **Trade-off:** deferring new areas keeps complexity lower now but leaves future extraction work.

## Deferred Area Extraction Path

This change intentionally keeps context/company orchestration in shared shell and shared controllers.

If workflows become complex enough to justify extraction, use this sequence:
1. Introduce dedicated `Customer` and `Company` areas while preserving current route contracts.
2. Move context-specific navigation and dashboards from shared entry experience into those areas.
3. Keep topbar context/company selector shared as the cross-area switch control.
4. Retain server-side context/company validation in shared switching endpoints so authorization logic does not drift.
5. Migrate tests from shared entry-focused assertions toward area-specific workflow assertions.

## Validation Strategy

- Manual and automated checks for role combinations:
  - company-only,
  - system-only,
  - company+system,
  - customer+company,
  - multi-company memberships.
- Verify unauthorized context/company selections are rejected and reset safely.
- Verify slug redirect correctness for company changes.

## Open Questions

- Whether active-company cookie stores `companyId` only or both `companyId` and `slug` for resilience.
- Whether context switch endpoint should preserve original return URL for in-place switching on eligible pages.
