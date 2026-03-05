## ADDED Requirements

### Requirement: Company workspace SHALL provide a Pricing & Products management surface
The system SHALL provide a dedicated Pricing & Products page (or clearly grouped subpages) in company workspace where authorized company operators manage tenant product and pricing configuration.

#### Scenario: Authorized operator opens Pricing & Products
- **WHEN** a signed-in CompanyAdmin or CompanyManager navigates company workspace operations
- **THEN** the UI shows a Pricing & Products entry and opens the management surface

#### Scenario: Unauthorized operator cannot manage pricing and products
- **WHEN** a signed-in company role without pricing permissions accesses the Pricing & Products route
- **THEN** access is denied by UI visibility and backend authorization rules

### Requirement: System SHALL support tenant box-size combinations for meal products
The system SHALL allow each tenant to configure product box-size combinations for 3, 4, and 5 meals paired with 2 or 4 people and persist those combinations as tenant-scoped product definitions.

#### Scenario: Tenant configures supported box combinations
- **WHEN** an authorized operator selects valid meal-count and people-count combinations and saves
- **THEN** the selected combinations are stored for that tenant and shown in subsequent management views

#### Scenario: Invalid box combination is rejected
- **WHEN** an operator attempts to save a combination outside allowed meal/person values
- **THEN** the system rejects the change and returns validation feedback

### Requirement: Product and pricing configuration SHALL be tenant-isolated
The system SHALL enforce tenant isolation for all Pricing & Products reads and writes so one tenant cannot view or modify another tenant's configuration.

#### Scenario: Tenant-scoped query returns only active tenant data
- **WHEN** an authorized operator loads Pricing & Products data
- **THEN** the system returns only records for the active tenant context

#### Scenario: Cross-tenant update attempt is denied
- **WHEN** a user submits an update referencing another tenant's pricing/product record
- **THEN** the system denies the operation and persists no cross-tenant change

