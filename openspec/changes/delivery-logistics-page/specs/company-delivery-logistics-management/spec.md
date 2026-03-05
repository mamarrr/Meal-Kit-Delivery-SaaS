## ADDED Requirements

### Requirement: Company Delivery Logistics page SHALL provide a unified operations workspace
The system SHALL provide a company-scoped Delivery Logistics page that consolidates delivery zones, zone schedules, deliveries/runs and orders by date, delivery tracking outcomes, and complaint-based escalation rules in a single operations workspace.

#### Scenario: Authorized company operator opens Delivery Logistics page
- **WHEN** a signed-in CompanyAdmin, CompanyManager, or CompanyEmployee navigates to the Delivery Logistics route for an active company context
- **THEN** the system shows a single Delivery Logistics page with sections for zones, schedules, runs/orders, tracking, and escalation

#### Scenario: Unauthorized company actor is denied access
- **WHEN** a user without a permitted company role attempts to open the Delivery Logistics route
- **THEN** the system denies access through UI and backend authorization checks

### Requirement: Delivery Logistics page SHALL enforce tenant-scoped data boundaries
All data displayed and mutated from the Delivery Logistics page MUST be filtered to the active company tenant and MUST NOT expose cross-tenant delivery operational data.

#### Scenario: Operator views logistics data in company context
- **WHEN** an authorized operator loads the Delivery Logistics page
- **THEN** only records associated with the active `CompanyId` are returned for each section

#### Scenario: Cross-tenant route tampering attempt
- **WHEN** a user attempts to access logistics records belonging to another company through parameter manipulation
- **THEN** the system rejects the request and does not disclose foreign-tenant records

