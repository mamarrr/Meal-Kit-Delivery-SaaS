## ADDED Requirements

### Requirement: EF Core repositories implement contracts
Each EF Core repository SHALL implement its corresponding repository interface from the DAL contracts layer.

#### Scenario: Repository implementation matches contract
- **WHEN** a repository interface is defined for an aggregate
- **THEN** an EF Core repository class implements that interface in the DAL EF layer

### Requirement: EF Core repositories inherit base EF repository
Repository implementations SHALL inherit from the shared base EF repository to provide consistent CRUD behavior.

#### Scenario: Repository implementation uses base EF repository
- **WHEN** a repository implementation is created
- **THEN** it derives from the base EF repository class for common data access behavior

### Requirement: Tenant filtering enforced in repository implementations
Repository implementations for tenant-scoped aggregates SHALL enforce CompanyId filtering in queries.

#### Scenario: Tenant-filtered query execution
- **WHEN** a repository method retrieves tenant-scoped entities
- **THEN** results are filtered by the current CompanyId

### Requirement: Update operations attach and update entities
Repository implementations SHALL explicitly attach and update existing entities to persist changes under no-tracking mode.

#### Scenario: Updating an existing entity
- **WHEN** a repository updates an existing entity
- **THEN** it calls the EF Core update method to ensure changes are saved
