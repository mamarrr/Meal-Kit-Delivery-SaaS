## Purpose

Define concrete BLL service implementations that fulfill service contracts, enforce tenant scoping, and are wired through DI for WebApp consumption.

## Requirements

### Requirement: Implement BLL services per contract
The system MUST implement a concrete BLL service class for each service interface, residing in the BLL implementation project and following the established naming conventions.

#### Scenario: Implementation class exists
- **WHEN** a developer inspects the BLL implementation project
- **THEN** each service interface has a corresponding concrete service class

### Requirement: Services orchestrate repositories and enforce tenant scoping
Service implementations MUST call repository interfaces to perform data access and MUST enforce tenant isolation by applying `CompanyId` scoping or tenant context in all queries and commands.

#### Scenario: Tenant scoping enforced in service
- **WHEN** a service method retrieves or mutates tenant-scoped data
- **THEN** it applies tenant context to ensure only the current tenant’s data is accessed

### Requirement: Services follow EF no-tracking update patterns
When updating existing entities, service implementations MUST ensure the repository uses explicit update semantics compatible with EF Core no-tracking behavior (e.g., attach and update before save).

#### Scenario: Update operation persists changes
- **WHEN** a service updates an existing entity
- **THEN** the update uses explicit attach/update semantics so changes are persisted

### Requirement: Service implementations enforce membership and role-management invariants
BLL service implementations SHALL validate actor authorization and tenant scope before executing add-by-email, role update, and membership removal operations, and SHALL enforce constraints around owner mutation paths.

#### Scenario: Service rejects add when account does not exist
- **WHEN** the add-by-email service operation is invoked with an email that has no corresponding platform account
- **THEN** the service returns a validation failure and does not create a company membership

#### Scenario: Service enforces tenant-scoped membership mutation
- **WHEN** a role update or removal operation targets a user or membership outside the active company context
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

### Requirement: Services registered for DI and consumed by WebApp
BLL services MUST be registered with dependency injection and consumed by the WebApp through BLL interfaces, including newly scaffolded-and-hardened controllers that expose required customer/company/system workflows.

#### Scenario: Services available via DI
- **WHEN** the WebApp resolves a BLL service from the service container
- **THEN** the concrete implementation is provided via its interface

#### Scenario: New workflow controller resolves required service dependencies
- **WHEN** a controller for a newly surfaced workflow is activated
- **THEN** all required BLL service dependencies resolve successfully through DI without direct repository access
