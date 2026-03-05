## ADDED Requirements

### Requirement: Tenant SHALL manage plan-tier pricing tables
The system SHALL provide tenant-scoped pricing tables for Free, Standard, and Premium plan tiers, allowing authorized operators to define and maintain pricing values for configured box products.

#### Scenario: Operator saves pricing values for a plan tier
- **WHEN** an authorized operator edits pricing values for a selected plan tier and saves
- **THEN** the system persists tenant-scoped pricing rows for that plan tier and box products

#### Scenario: Pricing table is shown grouped by plan tiers
- **WHEN** an authorized operator views Pricing & Products pricing tables
- **THEN** pricing entries are displayed in Free, Standard, and Premium sections for the active tenant

### Requirement: Pricing entries SHALL validate monetary and completeness constraints
The system SHALL validate that submitted pricing values are monetary amounts within allowed bounds and ensure required plan/box pricing entries are complete before activation.

#### Scenario: Invalid monetary value is rejected
- **WHEN** an operator submits a negative or malformed monetary value
- **THEN** the system rejects the save and returns a field-level validation error

#### Scenario: Incomplete required plan pricing set cannot be activated
- **WHEN** an operator attempts to activate pricing with missing required box entries for a plan tier
- **THEN** the system blocks activation and reports missing entries

### Requirement: Pricing tables SHALL preserve auditability of tenant changes
The system SHALL record who changed tenant pricing configuration and when, consistent with tenant audit trail policy.

#### Scenario: Pricing update stores actor and timestamp metadata
- **WHEN** an authorized operator updates plan pricing
- **THEN** persisted records include metadata sufficient to trace the actor and update time

