## 1. Workflow inventory and completion baseline

- [ ] 1.1 Build a workflow inventory from `openspec/config.yaml` and group it into customer-view, company-view, and system-view flows.
- [ ] 1.2 Create a workflow coverage matrix artifact (workflow → route/controller/action/view/roles/tenant-scope checks).
- [ ] 1.3 Define and document UI completion pass/fail criteria based on 100% workflow coverage.

## 2. Shared UI foundation

- [ ] 2.1 Refine shared layout, navigation, and page shell patterns for a professional visual baseline.
- [ ] 2.2 Implement reusable UI building blocks (form shells, list/table shells, action bars, alerts, empty states, validation summaries).
- [ ] 2.3 Apply accessibility/responsive baseline checks (keyboard access, focus states, readable contrast, tablet-safe layouts).

## 3. Controller scaffolding pipeline

- [ ] 3.1 Enumerate missing controllers needed to expose all required workflows.
- [ ] 3.2 Scaffold missing MVC controllers from inside WebApp using `dotnet aspnet-codegenerator controller` syntax from `README-TECH.md`.
- [ ] 3.3 Record scaffold commands and outcomes in AI evidence logs as required by project deliverables.

## 4. Controller hardening and architecture compliance

- [ ] 4.1 Replace scaffolded direct data-access patterns with App.Contracts.BLL service interface usage.
- [ ] 4.2 Add/verify role authorization attributes and tenant context enforcement in every tenant-owned action.
- [ ] 4.3 Convert complex controller views to typed view models (no ViewBag/ViewData) and align binding/validation patterns.
- [ ] 4.4 Verify update flows route through no-tracking-safe service/repository update paths.

## 5. Workflow implementation slices

- [ ] 5.1 Implement customer workflow slice pages/actions (subscription lifecycle, preferences, exclusions, selection windows, status tracking).
- [ ] 5.2 Implement company operations slice pages/actions (recipes, menus, categories, box config, pricing, zones, delivery windows).
- [ ] 5.3 Implement service/delivery quality slice pages/actions (delivery tracking, failed attempts, complaint handling, escalation-facing screens).
- [ ] 5.4 Implement system-level/admin-support slice pages/actions required for tenant/billing/support workflows.

## 6. Validation and evidence

- [ ] 6.1 Execute workflow-by-workflow verification against the coverage matrix and close all uncovered entries.
- [ ] 6.2 Run role/tenant isolation verification for all protected workflows and document results.
- [ ] 6.3 Run build/tests and targeted UI regression checks for newly added controller/view routes.
- [ ] 6.4 Update prompts/evidence documentation with commands, design decisions, and workflow completion proof.
