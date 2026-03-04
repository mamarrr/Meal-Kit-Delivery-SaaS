## Context

The current WebApp does not expose all target workflows through MVC controllers and production-ready UI flows. Existing domain, repository, and service layers are in place, but WebApp coverage is incomplete and inconsistent. The change must preserve multi-tenant isolation, role-based access boundaries, and the existing layered architecture while adding broad controller and UI surface area.

Key constraints:
- Layering remains strict: WebApp → App.BLL → App.DAL.EF → App.Domain.
- Tenant-owned operations must remain company-scoped.
- EF no-tracking behavior requires update flows to use explicit update semantics via established service/repository paths.
- Controller scaffolding must start with `dotnet aspnet-codegenerator` patterns from `README-TECH.md`, then be hardened.
- UI completion is judged by workflow coverage against `openspec/config.yaml` requirements.

Stakeholders:
- Product/tenant operators needing complete company workflows.
- End-customers needing complete customer subscription/preferences workflows.
- Engineering team needing maintainable and consistent controller/UI architecture.

## Goals / Non-Goals

**Goals:**
- Establish a repeatable controller delivery pipeline: scaffold first, then harden for project standards.
- Deliver workflow-complete UI coverage for customer and company contexts, including role-gated flows.
- Introduce a professional UI foundation with consistent layout, components, feedback, and responsive behavior.
- Create explicit workflow-coverage validation to determine completion.
- Keep service-contract-driven controller logic and avoid direct repository usage in WebApp.

**Non-Goals:**
- Re-architecting core domain entities or replacing existing layered architecture.
- Introducing a new frontend framework beyond current ASP.NET MVC/Razor stack.
- Replacing Identity subsystem design.
- Defining exhaustive visual brand system documentation beyond implementation-ready standards.

## Decisions

### Decision 1: Two-phase controller strategy (scaffold → harden)
- Choice: Generate initial controllers with `dotnet aspnet-codegenerator` and then refactor generated actions to match service contracts, tenant rules, update semantics, and view model conventions.
- Rationale: Maximizes delivery speed for broad workflow coverage while enforcing architecture/coding standards before final acceptance.
- Alternatives considered:
  - Manual controllers from scratch: higher consistency potential but slower and more error-prone for broad scope.
  - Keep scaffolded controllers mostly unchanged: faster initially but violates architecture and quality constraints.

### Decision 2: Workflow-first implementation ordering
- Choice: Implement controllers/views by workflow slices (e.g., recipe/menu/selection, delivery/complaints, subscription/customer management) rather than by technical layer alone.
- Rationale: Ensures measurable progress against UI completion criteria and reduces risk of partial, non-usable surfaces.
- Alternatives considered:
  - Entity-by-entity generic CRUD rollout: can produce disconnected user journeys and hidden workflow gaps.

### Decision 3: Explicit coverage matrix as release gate
- Choice: Maintain a coverage matrix mapping each required workflow to routes/controllers/views, role constraints, tenant checks, and validation criteria.
- Rationale: Converts vague “UI complete” definitions into verifiable acceptance criteria tied to config requirements.
- Alternatives considered:
  - Informal checklist in tasks only: weaker traceability and higher risk of missed workflows.

### Decision 4: Professional UI via shared building blocks
- Choice: Standardize page shell, form patterns, table/list patterns, status/alert patterns, and empty/error/loading states in shared layout/partials/components.
- Rationale: Delivers consistent and professional UX across many rapidly generated screens.
- Alternatives considered:
  - Per-page one-off styling: short-term flexibility but long-term inconsistency and maintenance cost.

### Decision 5: Preserve BLL contract boundary at controller layer
- Choice: Controllers consume App.Contracts.BLL services only; any required business behavior changes occur in BLL implementations and contracts, not in controller data-access logic.
- Rationale: Keeps architecture enforceable and testable; aligns with existing service specs and future refactoring.
- Alternatives considered:
  - Direct DAL repository calls from controllers: rejected due to boundary violations and coupling.

## Risks / Trade-offs

- [Risk] Scaffolded controllers may introduce anti-patterns (direct context/repository assumptions, weak validation). → Mitigation: enforce hardening checklist per controller before marking workflow complete.
- [Risk] Large number of workflows can lead to scope drift. → Mitigation: workflow matrix gate + iterative slice delivery + explicit out-of-scope tracking.
- [Risk] Role/tenant checks may be inconsistently applied across new actions. → Mitigation: standard authorization and tenant-scope review checklist embedded in controller acceptance criteria.
- [Risk] UI consistency can regress when many views are added quickly. → Mitigation: shared components/partials and design baseline tokens reused across all new pages.
- [Trade-off] Scaffold-first improves speed but adds refactor overhead. → Mitigation: accept overhead to accelerate initial surface creation while preserving final quality.

## Migration Plan

1. Finalize specs and tasks for controller, UI, and coverage capabilities.
2. Build/extend shared UI layout components and conventions first.
3. Scaffold targeted controllers by workflow groups using `dotnet aspnet-codegenerator`.
4. Harden controllers and views per group (BLL contracts, view models, role/tenant checks, no-tracking-safe update paths).
5. Populate and validate workflow coverage matrix as each group is completed.
6. Run end-to-end role/tenant workflow checks and finalize completion evidence.
7. Deploy with feature-scope verification in tenant-aware staging data.

Rollback strategy:
- If regressions appear, disable or remove affected new controller routes/views per workflow slice while preserving existing stable functionality.
- Revert individual workflow slices independently due to modular controller/view rollout.

## Open Questions

- Which exact workflow inventory from `openspec/config.yaml` should be grouped into implementation slices first (highest business priority ordering)?
- Should UI completion require all system-level workflows in the same milestone as customer/company workflows, or can system workflows be phased?
- Is an explicit Admin area now required for system-level operations, or should those workflows remain in main WebApp routing initially?
