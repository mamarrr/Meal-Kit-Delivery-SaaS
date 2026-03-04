## ADDED Requirements

### Requirement: Repository interfaces per aggregate
The system SHALL define a repository interface for each domain aggregate in the application data access contracts layer.

#### Scenario: Repository interface exists for aggregate
- **WHEN** a domain aggregate is present in the domain model
- **THEN** a corresponding repository interface exists in the DAL contracts layer

### Requirement: Repository interfaces inherit base repository contract
Each repository interface SHALL inherit from the base repository contract defined in the shared DAL contracts library.

#### Scenario: Repository interface extends base contract
- **WHEN** a repository interface is defined
- **THEN** it extends the shared base repository interface for common CRUD operations

### Requirement: Tenant-aware contracts for tenant-scoped aggregates
Repository interfaces for tenant-scoped aggregates SHALL expose query operations that apply CompanyId filtering.

#### Scenario: Tenant-scoped query contract
- **WHEN** a repository interface targets a tenant-scoped aggregate
- **THEN** its contract includes tenant-aware query methods or parameters that ensure CompanyId filtering

### Requirement: Repository contract surfaces no-tracking update expectations
Repository interfaces SHALL document that update operations require explicit entity updates to persist changes under no-tracking mode.

#### Scenario: Update semantics are defined
- **WHEN** a repository interface defines update operations
- **THEN** the contract specifies that explicit update calls are required for existing entities
