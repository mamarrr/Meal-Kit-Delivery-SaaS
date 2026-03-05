## ADDED Requirements

### Requirement: Service implementations enforce membership and role-management invariants
BLL service implementations SHALL validate actor authorization and tenant scope before executing add-by-email, role update, and membership removal operations, and SHALL enforce constraints around owner mutation paths.

#### Scenario: Service rejects add when account does not exist
- **WHEN** the add-by-email service operation is invoked with an email that has no corresponding platform account
- **THEN** the service returns a validation failure and does not create a company membership

#### Scenario: Service enforces tenant-scoped membership mutation
- **WHEN** a role update or removal operation targets a user/membership outside the active company context
- **THEN** the service rejects the operation and does not mutate data

#### Scenario: Service blocks standard owner mutation actions
- **WHEN** generic role update or member removal is invoked against the current owner account
- **THEN** the service rejects the operation and requires ownership transfer flow

### Requirement: Service implementations execute ownership transfer as a consistent state transition
BLL service implementations SHALL perform ownership transfer with validation that the actor is current owner and the target is an existing company member, and SHALL persist final role ownership state using explicit update semantics compatible with EF no-tracking behavior.

#### Scenario: Ownership transfer persists new owner state
- **WHEN** ownership transfer succeeds
- **THEN** the target member is persisted as CompanyOwner and previous owner is persisted as non-owner in one consistent transition

#### Scenario: Ownership transfer denied for non-owner actor
- **WHEN** a non-owner actor invokes ownership transfer service operation
- **THEN** the service denies the operation and leaves existing ownership unchanged

