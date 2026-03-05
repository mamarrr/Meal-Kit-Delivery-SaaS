## ADDED Requirements

### Requirement: Company ownership transfer SHALL be restricted to current owner
The system SHALL allow ownership transfer only when initiated by the current CompanyOwner in the active tenant context.

#### Scenario: Company admin attempts ownership transfer
- **WHEN** a CompanyAdmin initiates ownership transfer
- **THEN** the system denies the action
- **AND** ownership remains unchanged

### Requirement: Ownership transfer SHALL target an eligible tenant member
The system MUST require the transfer target to be an existing active member of the same company tenant before ownership can be transferred.

#### Scenario: Owner selects non-member as transfer target
- **WHEN** the CompanyOwner submits a transfer to a user who is not an active member of the tenant
- **THEN** the system rejects the request with validation feedback
- **AND** no role or ownership changes are persisted

### Requirement: Ownership transfer SHALL update tenant leadership atomically
When ownership transfer succeeds, the system MUST update tenant leadership so exactly one CompanyOwner remains and SHALL persist an auditable transfer record.

#### Scenario: Owner transfers ownership successfully
- **WHEN** the CompanyOwner confirms transfer to an eligible member
- **THEN** the target user becomes CompanyOwner in that tenant
- **AND** the previous owner is no longer CompanyOwner
- **AND** the transfer event is recorded with actor, target, tenant, and timestamp

