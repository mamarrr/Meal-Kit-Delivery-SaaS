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

### Requirement: Services registered for DI and consumed by WebApp
BLL services MUST be registered with dependency injection and consumed by the WebApp through BLL interfaces.

#### Scenario: Services available via DI
- **WHEN** the WebApp resolves a BLL service from the service container
- **THEN** the concrete implementation is provided via its interface
