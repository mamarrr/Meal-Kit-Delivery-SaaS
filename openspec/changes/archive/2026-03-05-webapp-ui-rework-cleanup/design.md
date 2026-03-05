## Context

`WebApp` currently contains a broad set of legacy MVC controllers, views, and view models that were built to cover a wide operational surface. The planned ground-up UI rework needs a controlled baseline first: only tenant/company registration, customer entry/registration paths, authentication/login flows, and slug-based tenant routing must remain operational while legacy UI surface is removed.

This is a cross-cutting cleanup because route endpoints, view model contracts, view files, and navigation links are intertwined. Existing specs also include broad UI expectations (including full workflow coverage) that no longer fit the rework-first strategy and must be updated to avoid conflicting requirements.

## Goals / Non-Goals

**Goals:**
- Establish a safe, explicit keep/remove boundary for `WebApp` UI cleanup.
- Preserve continuity of core user entry paths: company registration, customer-facing entry, and identity login flows.
- Preserve slug-based routing infrastructure as a stable foundation for future reworked pages.
- Align OpenSpec capability requirements with the temporary reduced UI scope.
- Minimize residual dead code and route exposure in the pre-rework codebase.

**Non-Goals:**
- Building replacement UI pages or introducing new UX in this change.
- Refactoring BLL/DAL/domain models beyond what is required by UI removal.
- Changing billing/subscription tier rules or tenant model semantics.
- Reworking Identity subsystem behavior beyond keeping required existing flows functioning.

## Decisions

1. **Use a whitelist retention strategy instead of blacklist deletion**
   - Decision: Define and preserve only explicitly required flow groups (company registration, customer essential entry/register paths, login/auth, slug resolution). Everything else in `WebApp` UI layer is eligible for removal in this change.
   - Rationale: Safer for a broad cleanup because inclusion criteria are narrow and testable.
   - Alternative considered: Incremental blacklist removal by “known legacy pages.” Rejected due to high risk of missing dead surface and inconsistent endpoint cleanup.

2. **Preserve slug-routing as infrastructure contract**
   - Decision: Treat slug resolution/routing middleware/helpers/transforms as protected assets even if many consuming pages are removed.
   - Rationale: The ground-up UI must reuse tenant context routing immediately; removing or weakening slug logic would create rework churn.
   - Alternative considered: Temporarily collapsing to non-slug routes. Rejected because it breaks multi-tenant path requirements and introduces migration debt.

3. **Update specs in parallel with cleanup boundary definition**
   - Decision: Introduce a dedicated cleanup-boundary capability spec and add deltas to existing related specs.
   - Rationale: Prevents implementation/spec drift and makes future apply work deterministic.
   - Alternative considered: Keep specs unchanged and rely on implementation notes. Rejected because old broad requirements would conflict with intended cleanup.

4. **Perform route-safety validation after removals**
   - Decision: Validate that preserved routes still resolve and removed routes no longer appear in navigation/endpoints.
   - Rationale: Deleting controllers/views can produce broken links and ambiguous runtime behavior if not verified.
   - Alternative considered: File-only deletion without route verification. Rejected due to high regressions risk.

## Risks / Trade-offs

- **[Risk] Over-deletion accidentally removes required auth/onboarding path** → Mitigation: Maintain explicit retained-flow inventory and run focused route/auth smoke checks.
- **[Risk] Under-deletion leaves substantial legacy UI surface** → Mitigation: Perform namespace/folder-level audit of controllers/viewmodels/views against retention whitelist.
- **[Risk] Navigation/layout references removed endpoints** → Mitigation: Update shared layout/nav partials as part of cleanup and verify no stale links remain.
- **[Trade-off] Temporary reduced functionality while rework is in progress** → Mitigation: Document this as intentional and constrained by updated specs.

## Migration Plan

1. Build retained-flow inventory (controllers/actions/views/view models + slug/auth dependencies).
2. Remove non-whitelisted UI components from `WebApp` (controllers, view models, views, related routing references).
3. Update shared navigation and entry points to include only retained paths.
4. Run targeted validation for login, company registration, customer entry/registration, and slug route resolution.
5. Update specs and tasks to encode cleanup boundary and preserved behaviors.
6. Rollback strategy: restore deleted UI components from VCS if retained-flow validation fails; re-apply cleanup with corrected whitelist.

## Open Questions

- Which exact customer-facing flows are considered mandatory “registration-adjacent” for this interim period (e.g., subscription selection vs. account-only onboarding)?
- Should `workflow-complete-web-ui` be narrowed via strict requirement reduction or partially superseded by the new cleanup-boundary capability?
- Do we want explicit temporary deprecation notes for removed legacy routes to help downstream consumers/test scripts?
