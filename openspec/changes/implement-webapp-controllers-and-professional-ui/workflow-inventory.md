# Workflow Inventory

This inventory is derived from [`openspec/config.yaml`](openspec/config.yaml) and grouped equally across customer, company, and system views.

## Customer View Workflows

| Workflow ID | Workflow | Key User Outcome |
|---|---|---|
| CUST-01 | Discover available company meal-kit plans | Customer can identify a company and subscription option to join |
| CUST-02 | Subscribe to company meal-kit service | Customer can start subscription lifecycle with selected box and tier |
| CUST-03 | Manage subscription status | Customer can pause/resume/cancel and view active status |
| CUST-04 | Set dietary preferences | Customer can define dietary categories (omnivore/vegetarian/pescatarian/vegan) |
| CUST-05 | Set ingredient exclusions | Customer can define exclusions (e.g. shellfish/pork/cilantro) |
| CUST-06 | Review weekly menu options | Customer can view current week recipes and choices |
| CUST-07 | Submit meal selections before deadline | Customer can choose recipes by configured selection cutoff |
| CUST-08 | Receive auto-selection fallback | Customer gets auto-selection if no manual choice by deadline |
| CUST-09 | Enforce non-repetition window | Customer does not receive repeated meals within past N weeks |
| CUST-10 | Apply nutritional filtering | Customer can filter by calories/macros/fiber/sodium |
| CUST-11 | Track delivery status | Customer can view delivery progress and outcome |
| CUST-12 | Submit quality complaint | Customer can submit issue for missed/late/damaged/quality problems |

## Company View Workflows

| Workflow ID | Workflow | Key User Outcome |
|---|---|---|
| COMP-01 | Manage company profile/settings | Company users can configure tenant identity and settings |
| COMP-02 | Configure box sizes | Company configures 3/4/5 meals × 2/4 people combinations |
| COMP-03 | Configure pricing | Company defines and maintains pricing model |
| COMP-04 | Manage dietary categories | Company manages selectable dietary category catalog |
| COMP-05 | Manage recipe pool | Company CRUDs recipe definitions and metadata |
| COMP-06 | Maintain nutritional data | Company stores per-serving calories/protein/carbs/fat/fiber/sodium |
| COMP-07 | Build weekly menus | Company publishes weekly recipe lineup by categories |
| COMP-08 | Configure selection deadlines | Company controls weekly menu choice cutoff |
| COMP-09 | Manage delivery zones | Company defines zones and constraints |
| COMP-10 | Manage delivery day/time windows | Company maintains delivery schedules per zone |
| COMP-11 | Track delivery attempts | Employees can log success/failure and attempt history |
| COMP-12 | Handle quality complaints | Staff triages and resolves complaints |
| COMP-13 | Apply complaint escalation rules | Repeated complaints trigger escalation workflow |
| COMP-14 | Manage tenant users and roles | Admin/owner manage CompanyAdmin/Manager/Employee assignments |
| COMP-15 | Observe free-tier operational limits | Tenant enforces 1 zone / 50 subscribers for free plan |

## System View Workflows

| Workflow ID | Workflow | Key User Outcome |
|---|---|---|
| SYS-01 | Manage companies | System admin can create/update/deactivate tenant companies |
| SYS-02 | Manage subscription plans and tiers | Billing/admin users maintain Free/Standard/Premium model |
| SYS-03 | Manage billing and invoices | Billing users manage invoice/payment lifecycle |
| SYS-04 | Configure system-level options | System admin manages global configuration and feature flags |
| SYS-05 | View cross-tenant analytics | System admin gets aggregated platform insights |
| SYS-06 | Impersonate company users for support | System admin can troubleshoot tenant context issues |
| SYS-07 | View-only support access | Support can inspect tenant data without mutating billing/config |
| SYS-08 | Manage support ticketing | Support can create and track tickets |

## Cross-Cutting Workflow Constraints

- Data isolation: every tenant-owned query/action must be company-scoped.
- Role authorization: workflow entry/action availability follows declared role boundaries.
- Single-account multi-context behavior: one identity can act in customer and company views.
- No-tracking update path safety: mutation workflows route through explicit update semantics in service/repository layers.
