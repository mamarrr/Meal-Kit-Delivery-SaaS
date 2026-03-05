## ADDED Requirements

### Requirement: Company owner can transfer ownership to an existing company member
The system SHALL provide a company-scoped ownership transfer action that is available only to the current CompanyOwner and allows transfer only to a user who is already a member of the same company.

#### Scenario: Owner transfers ownership to company admin
- **WHEN** the current CompanyOwner selects an existing company member and confirms ownership transfer
- **THEN** the selected member becomes CompanyOwner for that company
- **AND** the previous owner is reassigned to a non-owner company role

#### Scenario: Reject transfer target outside company membership
- **WHEN** the current CompanyOwner attempts to transfer ownership to a user who is not a member of the active company
- **THEN** the system denies the transfer and keeps the original owner unchanged

#### Scenario: Reject non-owner attempt to transfer ownership
- **WHEN** a CompanyAdmin, CompanyManager, or CompanyEmployee attempts to initiate ownership transfer
- **THEN** the system denies the operation

### Requirement: Ownership transfer preserves a valid single-owner state
The system MUST enforce ownership invariants so that ownership transfer is applied as a single consistent operation that does not leave the company without an owner.

#### Scenario: Ownership remains valid during transfer
- **WHEN** ownership transfer is executed
- **THEN** the system persists a state with exactly one CompanyOwner in the company after the operation completes
