## Context

Company users need a tenant-scoped operational page to manage subscriber accounts without switching across multiple workflow screens. The new change introduces a **Subscribers & Accounts** page in company workspaces with list filtering and comprehensive detail panes (addresses, zone, preferences, exclusions, plan/box, ratings/history, and pause/skip/cancellation lifecycle visibility).

Current architecture already has layered separation (WebApp → BLL → DAL) and domain entities for customer/subscription/menu concepts, but no consolidated company-facing subscriber operations surface defined as a single workflow capability. The design must preserve strict tenant isolation (`CompanyId` scope), role-based access behavior, and MVC ViewModel patterns.

## Goals / Non-Goals

**Goals:**
- Provide one company workspace page with integrated list + details for subscriber account operations.
- Support list search and filtering by status, tier, and delivery zone.
- Expose subscriber profile context required for operations: addresses, delivery zone, preferences/exclusions, plan/box, ratings history, and lifecycle events (pauses/skips/cancellations).
- Keep all data reads tenant-scoped and role-authorized for company roles.
- Fit cleanly into existing MVC/BLL/DAL contracts with reusable query methods.

**Non-Goals:**
- Building new customer-facing account pages.
- Changing platform billing/subscription tier definitions.
- Reworking recipe auto-selection algorithms (only exposing ratings/history context for operational visibility).
- Introducing cross-tenant support/system-level management workflows.

## Decisions

1. **Introduce a dedicated company workspace route and page composition**
   - Decision: Add a new company route/page for Subscribers & Accounts, with a split experience: filterable list and in-page detail sections.
   - Rationale: Meets operational need for low-friction triage and management without multi-page navigation.
   - Alternatives considered:
     - Extend existing generic customer list pages: rejected due to fragmented UX and missing operational sections.
     - Build multiple subpages only: rejected because it increases navigation overhead for support and operations teams.

2. **Use projection-focused ViewModels for list and detail payloads**
   - Decision: Build dedicated ViewModels for search/filter state, list rows, and detail panels instead of passing domain entities directly.
   - Rationale: Aligns with project guideline to use ViewModels for composite UI data and avoids over-fetching.
   - Alternatives considered:
     - Pass domain entities directly: rejected because page requires multi-aggregate data and UI-specific shaping.

3. **Add/extend BLL and repository query methods for tenant-scoped aggregation**
   - Decision: Implement service/repository methods for:
     - filtered subscriber list retrieval (search/status/tier/zone)
     - selected subscriber detail retrieval (addresses, zone, preferences/exclusions, plan/box, ratings/lifecycle history)
   - Rationale: Keeps controller thin and encapsulates query behavior in service/repository layers.
   - Alternatives considered:
     - Query directly in controller: rejected as it violates layering and reduces testability.

4. **Preserve strict tenant filtering and role authorization at every layer**
   - Decision: Enforce tenant scope via current-company context and repository filters on all list/detail queries; protect endpoints with company role authorization.
   - Rationale: Meets core multi-tenant isolation requirement and prevents accidental data leakage.
   - Alternatives considered:
     - UI-only tenant filtering: rejected as insufficient security boundary.

5. **Treat history sections as read-oriented timeline blocks in first iteration**
   - Decision: Ratings history and pause/skip/cancellation history are rendered as read-focused sections to support operational decisions; action workflows remain separate unless explicitly required later.
   - Rationale: Delivers core visibility quickly while minimizing scope risk.
   - Alternatives considered:
     - Full inline editing/actions for all lifecycle states: deferred to avoid excessive first-scope complexity.

## Risks / Trade-offs

- **[Risk]** Query complexity and potential performance issues from multi-section detail aggregation.  
  **Mitigation:** Use projection queries, pagination for list, and deferred/lazy loading patterns for heavy history segments where needed.

- **[Risk]** Inconsistent tenant filtering across newly added service/repository methods.  
  **Mitigation:** Centralize company context access and require tenant predicate checks in all related repository query methods.

- **[Risk]** UI over-density from too many sections on a single page.  
  **Mitigation:** Use clearly segmented panels/tabs/accordions while keeping key summary fields visible by default.

- **[Trade-off]** Read-first history sections reduce immediate operational actionability.  
  **Mitigation:** Add follow-up changes for inline lifecycle actions after validating usage and performance.

## Migration Plan

1. Add/extend company-scope service and repository contracts for list/detail query operations.
2. Implement DAL query projections for subscriber list filters and detail sections.
3. Add controller actions and ViewModels for the new company page route.
4. Build Razor UI for list filters/results and detail section rendering.
5. Validate tenant isolation, role access, and filter behavior with integration tests/manual verification.
6. Deploy with feature included in company workspace navigation.

Rollback strategy:
- Remove navigation link and disable route usage while retaining database/data model unchanged (this change is primarily query/UI oriented).
- Revert controller/service/view changes if regressions are detected.

## Open Questions

- Should company roles have differentiated visibility (e.g., CompanyEmployee sees fewer financial/plan details than CompanyAdmin/Manager)?
- Should ratings history include only meal ratings, or also auto-selection rationale snapshots?
- Is list pagination size fixed globally or configurable per company workspace?
- Do pauses/skips/cancellations require immediate inline actions in this same change, or remain view-only for now?
