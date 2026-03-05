## Why

Company managers need a single place to plan, configure, and validate weekly menus, but current workflows are split across recipe/menu data and do not provide a scheduling-first experience. This change introduces a dedicated Menus & Scheduling area so companies can reliably publish balanced weekly menus and enforce deadline/no-repeat rules as subscriber volume grows.

## What Changes

- Add a new Company workspace page for **Menus & Scheduling** with either one unified screen or linked subpages for planning tasks.
- Add weekly menu calendar/week-view planning capabilities for selecting a target week and seeing assigned recipes by category.
- Add menu configuration capabilities for per-category recipe counts, subscriber choice deadline rules, and configurable "no-repeat N weeks" policy.
- Add recipe-to-week assignment workflow with validation against category quotas and repeat-window constraints.
- Add auto-selection rules preview/simulation to test how rules would select recipes for a week (optional in UX depth, required as capability scaffold).
- Integrate the page into company navigation/authorization for company operational roles.

## Capabilities

### New Capabilities
- `company-menus-and-scheduling-management`: Manage weekly menu scheduling, menu configuration rules, and recipe assignment lifecycle in company view.
- `menu-auto-selection-simulation`: Preview/simulate weekly auto-selection outcomes against configuration constraints before publishing.

### Modified Capabilities
- `workflow-complete-web-ui`: Extend company UI workflow coverage to include Menus & Scheduling entry, navigation, and role-appropriate access.

## Impact

- Affected systems: company-facing MVC UI, menu-related BLL services/contracts, and menu data/repository queries needed for week-based planning.
- Likely affected code areas: `WebApp/Controllers/`, `WebApp/Views/`, navigation layout, menu service/repository interfaces and implementations in `App.Contracts.BLL`, `App.BLL`, `App.Contracts.DAL`, and `App.DAL.EF`.
- Data and domain impact: may require additional weekly planning/configuration fields and validation logic; ensure tenant-scoped access and existing no-tracking EF update patterns are respected.
- No external API breaking change expected for current scope; behavior expands within company UI and internal service contracts as needed.
