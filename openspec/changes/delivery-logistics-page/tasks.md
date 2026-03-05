## 1. Delivery Logistics domain and contracts

- [x] 1.1 Inventory existing delivery entities/services/repositories and map gaps against logistics specs (zones/schedules/capacity, runs/orders by date, tracking, escalation).
- [x] 1.2 Extend BLL and DAL interfaces for missing queries/commands, including date-filtered run+order retrieval and escalation rule read/write operations.
- [x] 1.3 Implement repository/service updates for schedule and tracking writes with explicit `Update()` semantics under no-tracking DbContext behavior.

## 2. Company Delivery Logistics UI surface

- [x] 2.1 Add/extend company operations controller routes for Delivery Logistics page and section-level post actions.
- [x] 2.2 Create strongly typed composite and section ViewModels for zones list, schedules/capacity, runs/orders, tracking state, and escalation rules.
- [x] 2.3 Build the Delivery Logistics Razor view as one page with clearly separated sections and validation feedback for each update workflow.

## 3. Authorization, tenancy, and navigation integration

- [x] 3.1 Enforce role-based access so CompanyAdmin/CompanyManager/CompanyEmployee can view page, while edit actions are constrained per policy.
- [x] 3.2 Ensure all delivery logistics reads/writes are tenant-scoped by active `CompanyId`, including tamper-resistant id/route checks.
- [x] 3.3 Add Delivery Logistics entry into company navigation and ensure route follows existing slug/company context conventions.

## 4. Escalation policy and tracking behavior

- [x] 4.1 Implement complaint-based escalation rule persistence and deterministic priority evaluation inputs (freshest stock, earliest slot).
- [x] 4.2 Add delivery tracking updates for success/failed attempt, notes, proof metadata, and reschedule fields with audit-friendly timestamps.
- [x] 4.3 Validate schedule windows/capacity and escalation inputs with server-side rules and user-visible error messages.

## 5. Verification and readiness

- [x] 5.1 Add/adjust unit tests for service-layer tenant filtering, update persistence, and escalation rule behavior.
- [x] 5.2 Add/adjust web/integration tests for authorized/unauthorized access, date filtering, and one-page workflow rendering.
- [x] 5.3 Run full build/tests, capture remaining implementation risks, and update prompts evidence/logging documentation as required.
