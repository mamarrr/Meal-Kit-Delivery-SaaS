## Context

The product is a multi-tenant MVC web app where company context is conveyed through path-based slug routing. Current operational pages are not consistently slug-scoped, causing mixed URL shapes, fragile deep links, and increased risk of cross-context user confusion. This change introduces a single canonical route contract for system, company, and customer operations so all links and redirects preserve tenant context predictably.

Stakeholders include platform engineering (routing consistency), support (reliable deep links for troubleshooting), and tenant administrators/end users (clear company-scoped navigation).

## Goals / Non-Goals

**Goals:**
- Define one canonical route strategy for operational pages: `/{slug}/{OperationPage}` (or route-equivalent with area/controller/action segments where needed).
- Ensure slug resolution and authorization checks run before operational actions execute.
- Ensure all UI navigation and post-auth redirects generate slug-scoped URLs.
- Preserve compatibility through explicit legacy route handling (301/302 redirects) for previously unscoped operations URLs.
- Provide testable acceptance paths for system, company, and customer operations surfaces.

**Non-Goals:**
- Reworking domain entities, repository contracts, or data model shape.
- Redesigning UI components/styles beyond link and route targets.
- Introducing new identity provider behavior or role model changes.
- Removing old routes immediately without transition safeguards.

## Decisions

1. **Canonical routing contract uses slug-first path segments for all operations pages.**  
   - **Decision:** Adopt slug-prefixed route templates for operations controllers/pages and require generated links to include slug values.  
   - **Rationale:** Aligns with multi-tenancy path requirements and prevents non-deterministic route patterns.  
   - **Alternatives considered:**
     - Query-string tenant context (`?company=slug`) — rejected due to weaker URL semantics and easier accidental context loss.
     - Header/session-only tenant context — rejected because deep links become opaque and less shareable.

2. **Centralized slug resolution and guard behavior remains the source of tenant context truth.**  
   - **Decision:** Keep tenant resolution in middleware/provider and enforce that operations endpoints validate slug existence and active tenant access before executing business logic.  
   - **Rationale:** Avoids duplicating tenant lookup logic in each controller; ensures consistent failure behavior (404 for unknown slug, forbidden/redirect for unauthorized scope).  
   - **Alternatives considered:**
     - Per-controller custom slug lookup — rejected due to duplication and drift risk.

3. **Legacy route compatibility handled via explicit redirect mapping.**  
   - **Decision:** Maintain temporary non-slug route entry points that redirect to canonical slug URLs when tenant can be resolved safely.  
   - **Rationale:** Prevents bookmark breakage and reduces rollout risk for active users.  
   - **Alternatives considered:**
     - Hard cutover with immediate removal — rejected due to support burden and avoidable UX disruption.

4. **Navigation and post-auth flows must always emit slug-aware links.**  
   - **Decision:** Update shared navigation view models/link generation to carry slug for all operations destinations, and ensure post-login landing resolves to slug-scoped operations route.  
   - **Rationale:** URL consistency must be guaranteed at generation time, not only accepted at endpoint time.  
   - **Alternatives considered:**
     - Allow mixed link generation and rely on redirects — rejected as it prolongs inconsistency and complicates analytics.

## Risks / Trade-offs

- **[Risk] Route collisions or ambiguities with existing controller/action templates** → **Mitigation:** Introduce route naming conventions and add integration tests for representative operations endpoints across system/company/customer scopes.
- **[Risk] Incorrect slug propagation in shared partials/navigation** → **Mitigation:** Centralize slug in app-shell navigation model and validate generated hrefs in view/integration tests.
- **[Risk] Redirect loops during legacy URL migration** → **Mitigation:** Ensure redirect targets are canonicalized once and add regression tests for old-to-new URL transitions.
- **[Risk] Unauthorized slug access attempts may expose behavior inconsistencies** → **Mitigation:** Standardize failure responses for unknown slug vs unauthorized slug and verify via authorization tests.
- **[Trade-off] Temporary dual-route support increases complexity short-term** → **Mitigation:** Time-box legacy route support and track removal in follow-up cleanup tasks.

## Migration Plan

1. Inventory operational routes currently lacking slug prefixes.
2. Introduce/standardize slug-prefixed route templates and ensure link generation includes slug.
3. Add compatibility redirects from known legacy routes to canonical slug routes.
4. Update post-auth entry routing and app-shell navigation to canonical URLs.
5. Run integration tests for routing, authorization, and navigation link correctness.
6. Deploy with monitoring for redirect volumes and route errors; rollback by disabling redirect map and reverting route registrations if severe regressions occur.

## Open Questions

- Should system operations use a dedicated reserved slug (e.g., `system`) or continue using company/user-resolved slug semantics for every page?
- For users associated with multiple companies, what is the deterministic default slug immediately after authentication when no explicit company choice is made?
- What is the target deprecation window for legacy non-slug routes before permanent removal?
