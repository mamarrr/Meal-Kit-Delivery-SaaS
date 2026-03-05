## 1. Domain and persistence extensions for weekly scheduling

- [x] 1.1 Add/extend domain entities and EF mappings for weekly menu configuration (category recipe counts, deadline policy, no-repeat weeks) in tenant scope.
- [x] 1.2 Add/extend repositories and DAL contracts for week-based menu planning queries and recipe assignment operations.
- [x] 1.3 Add migration(s) for new scheduling/configuration data with safe defaults and backward-compatible constraints.

## 2. Business logic and rule engine implementation

- [x] 2.1 Extend BLL contracts and services for retrieving/saving weekly menu configuration.
- [x] 2.2 Implement assignment validation for category quotas and no-repeat N-weeks constraints with clear validation outcomes.
- [x] 2.3 Implement auto-selection simulation service that performs dry-run candidate selection and returns exclusion diagnostics.

## 3. Company UI: Menus & Scheduling workflow

- [x] 3.1 Add Menus & Scheduling controller/actions and strongly typed view models for week view, configuration, assignment, and simulation result display.
- [x] 3.2 Implement Razor views (single page or subpages) for weekly calendar/week view, configuration form, assignment flow, and simulation preview.
- [x] 3.3 Add company navigation entry and role-based access controls for CompanyOwner, CompanyAdmin, and CompanyManager.

## 4. Integration hardening and validation

- [x] 4.1 Ensure tenant isolation is enforced on all scheduling queries/commands and add guard checks for company context.
- [x] 4.2 Verify update paths explicitly call update operations under EF no-tracking behavior.
- [x] 4.3 Add/extend automated tests (repository/service/controller) for rule enforcement, authorization boundaries, and simulator no-side-effect guarantees.

## 5. Delivery readiness

- [x] 5.1 Validate end-to-end company workflow from navigation to week planning, assignment, and simulation.
- [x] 5.2 Update documentation/changelog entries for new Menus & Scheduling capability and operational behavior.
