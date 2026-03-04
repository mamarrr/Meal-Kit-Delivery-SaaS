# Workflow Coverage Matrix

Source inventory: [`workflow-inventory.md`](openspec/changes/implement-webapp-controllers-and-professional-ui/workflow-inventory.md)

## Usage

- One row per workflow from the inventory.
- Mark `Status` as `Not Started`, `In Progress`, `Covered`, or `Blocked`.
- `Covered` requires implemented route/controller/action/view plus validated role + tenant checks and evidence links.
- Any row not `Covered` means UI completion is not yet achieved.

## Matrix

| Workflow ID | View Context | Workflow | Route | Controller | Action(s) | View/Component | Allowed Roles | Tenant Scope Check | Feedback Pattern (success/validation/error) | Evidence | Status |
|---|---|---|---|---|---|---|---|---|---|---|---|
| CUST-01 | Customer | Discover available company meal-kit plans | TBD | TBD | TBD | TBD | Customer / Authenticated | N/A (discovery) | TBD | TBD | Not Started |
| CUST-02 | Customer | Subscribe to company meal-kit service | TBD | TBD | TBD | TBD | Customer | Must bind subscription to current user and selected tenant | TBD | TBD | Not Started |
| CUST-03 | Customer | Manage subscription status | TBD | TBD | TBD | TBD | Customer | Must restrict subscription records to current user + tenant | TBD | TBD | Not Started |
| CUST-04 | Customer | Set dietary preferences | TBD | TBD | TBD | TBD | Customer | Must scope preferences to current user + tenant | TBD | TBD | Not Started |
| CUST-05 | Customer | Set ingredient exclusions | TBD | TBD | TBD | TBD | Customer | Must scope exclusions to current user + tenant | TBD | TBD | Not Started |
| CUST-06 | Customer | Review weekly menu options | TBD | TBD | TBD | TBD | Customer | Must scope menus to subscription tenant | TBD | TBD | Not Started |
| CUST-07 | Customer | Submit meal selections before deadline | TBD | TBD | TBD | TBD | Customer | Must scope selection to own subscription + deadline | TBD | TBD | Not Started |
| CUST-08 | Customer | Receive auto-selection fallback | TBD | TBD | TBD | TBD | Customer / System Process | Must scope fallback generation to tenant rules | TBD | TBD | Not Started |
| CUST-09 | Customer | Enforce non-repetition window | TBD | TBD | TBD | TBD | Customer / System Process | Must enforce tenant-configured N-week rule | TBD | TBD | Not Started |
| CUST-10 | Customer | Apply nutritional filtering | TBD | TBD | TBD | TBD | Customer | Must query tenant-visible recipe/menu set only | TBD | TBD | Not Started |
| CUST-11 | Customer | Track delivery status | TBD | TBD | TBD | TBD | Customer | Must show only deliveries bound to customer + tenant | TBD | TBD | Not Started |
| CUST-12 | Customer | Submit quality complaint | TBD | TBD | TBD | TBD | Customer | Must bind complaint to customer delivery and tenant | TBD | TBD | Not Started |
| COMP-01 | Company | Manage company profile/settings | TBD | TBD | TBD | TBD | CompanyOwner / CompanyAdmin | Must enforce current tenant ownership | TBD | TBD | Not Started |
| COMP-02 | Company | Configure box sizes | TBD | TBD | TBD | TBD | CompanyAdmin / CompanyManager | Must scope box config by tenant | TBD | TBD | Not Started |
| COMP-03 | Company | Configure pricing | TBD | TBD | TBD | TBD | CompanyOwner / CompanyAdmin | Must scope pricing by tenant + plan limits | TBD | TBD | Not Started |
| COMP-04 | Company | Manage dietary categories | TBD | TBD | TBD | TBD | CompanyAdmin / CompanyManager | Must scope category catalog by tenant | TBD | TBD | Not Started |
| COMP-05 | Company | Manage recipe pool | TBD | TBD | TBD | TBD | CompanyAdmin / CompanyManager / CompanyEmployee (limited) | Must scope recipes by tenant and role capabilities | TBD | TBD | Not Started |
| COMP-06 | Company | Maintain nutritional data | TBD | TBD | TBD | TBD | CompanyAdmin / CompanyManager | Must scope nutrition records to tenant recipes | TBD | TBD | Not Started |
| COMP-07 | Company | Build weekly menus | TBD | TBD | TBD | TBD | CompanyAdmin / CompanyManager | Must scope menu lifecycle to current tenant | TBD | TBD | Not Started |
| COMP-08 | Company | Configure selection deadlines | TBD | TBD | TBD | TBD | CompanyAdmin / CompanyManager | Must scope deadline rules to tenant menus | TBD | TBD | Not Started |
| COMP-09 | Company | Manage delivery zones | TBD | TBD | TBD | TBD | CompanyOwner / CompanyAdmin | Must scope zones to tenant; enforce tier limits | TBD | TBD | Not Started |
| COMP-10 | Company | Manage delivery day/time windows | TBD | TBD | TBD | TBD | CompanyAdmin / CompanyManager | Must scope windows to tenant zones | TBD | TBD | Not Started |
| COMP-11 | Company | Track delivery attempts | TBD | TBD | TBD | TBD | CompanyEmployee / CompanyManager / CompanyAdmin | Must scope delivery operations to tenant assignments | TBD | TBD | Not Started |
| COMP-12 | Company | Handle quality complaints | TBD | TBD | TBD | TBD | CompanyEmployee / CompanyManager / CompanyAdmin | Must scope complaints to tenant and allowed queues | TBD | TBD | Not Started |
| COMP-13 | Company | Apply complaint escalation rules | TBD | TBD | TBD | TBD | CompanyManager / CompanyAdmin | Must scope escalation rules and targets to tenant | TBD | TBD | Not Started |
| COMP-14 | Company | Manage tenant users and roles | TBD | TBD | TBD | TBD | CompanyOwner / CompanyAdmin | Must restrict role assignment within current tenant | TBD | TBD | Not Started |
| COMP-15 | Company | Observe free-tier operational limits | TBD | TBD | TBD | TBD | CompanyOwner / CompanyAdmin | Must enforce feature/entity caps by tenant plan | TBD | TBD | Not Started |
| SYS-01 | System | Manage companies | TBD | TBD | TBD | TBD | SystemAdmin | Cross-tenant allowed by system role policy | TBD | TBD | Not Started |
| SYS-02 | System | Manage subscription plans and tiers | TBD | TBD | TBD | TBD | SystemAdmin / SystemBilling | Cross-tenant allowed by system role policy | TBD | TBD | Not Started |
| SYS-03 | System | Manage billing and invoices | TBD | TBD | TBD | TBD | SystemBilling | Cross-tenant allowed by system role policy | TBD | TBD | Not Started |
| SYS-04 | System | Configure system-level options | `/SystemSettings` | `SystemSettingsController` | `Index/Create/Edit/Delete/Details` | `Views/SystemSettings/*` | SystemAdmin | Cross-tenant allowed by system role policy | Scaffolded CRUD feedback patterns present in Razor views | `App.Domain/Support/SystemSetting.cs`; `WebApp/Controllers/SystemSettingsController.cs`; `App.DAL.EF/Migrations/20260304155325_SupportSystemWorkflows.cs` | In Progress |
| SYS-05 | System | View cross-tenant analytics | `/Analytics` | `AnalyticsController` | `Index/Create/Edit/Delete/Details` | `Views/Analytics/*` | SystemAdmin | Cross-tenant allowed by system role policy | Scaffolded CRUD feedback patterns present in Razor views | `App.Domain/Support/SystemAnalyticsSnapshot.cs`; `WebApp/Controllers/AnalyticsController.cs`; `App.DAL.EF/Migrations/20260304155325_SupportSystemWorkflows.cs` | In Progress |
| SYS-06 | System | Impersonate company users for support | `/SupportImpersonation` | `SupportImpersonationController` | `Index/Create/Edit/Delete/Details` | `Views/SupportImpersonation/*` | SystemAdmin | Requires auditable impersonation boundary checks | Scaffolded CRUD feedback patterns present in Razor views | `App.Domain/Support/SupportImpersonationSession.cs`; `WebApp/Controllers/SupportImpersonationController.cs`; `App.DAL.EF/Migrations/20260304155325_SupportSystemWorkflows.cs` | In Progress |
| SYS-07 | System | View-only support access | `/SupportAccess` | `SupportAccessController` | `Index/Create/Edit/Delete/Details` | `Views/SupportAccess/*` | SystemSupport | Must deny mutation and billing/config edits | Scaffolded CRUD feedback patterns present in Razor views | `App.Domain/Support/TenantSupportAccess.cs`; `WebApp/Controllers/SupportAccessController.cs`; `App.DAL.EF/Migrations/20260304155325_SupportSystemWorkflows.cs` | In Progress |
| SYS-08 | System | Manage support ticketing | `/SupportTickets` | `SupportTicketsController` | `Index/Create/Edit/Delete/Details` | `Views/SupportTickets/*` | SystemSupport / SystemAdmin | Must enforce ticket scope and immutable audit trail | Scaffolded CRUD feedback patterns present in Razor views | `App.Domain/Support/SupportTicket.cs`; `App.Domain/Support/SupportTicketStatus.cs`; `WebApp/Controllers/SupportTicketsController.cs`; `App.DAL.EF/Migrations/20260304155325_SupportSystemWorkflows.cs` | In Progress |

## UI Completion Gate (Pass/Fail)

UI is **PASS** only when all matrix rows are `Covered` and each covered row includes:

1. Implemented route/controller/action/view mapping.
2. Role expectation that matches workflow authorization requirements.
3. Tenant-scope expectation and verification notes (or explicit N/A for system cross-tenant workflows).
4. Evidence links to implementation artifacts and validation checks.
5. Feedback behavior for success, validation failure, and error conditions.

UI is **FAIL** if any required workflow row is missing, blocked, or lacks role/tenant/evidence completeness.
