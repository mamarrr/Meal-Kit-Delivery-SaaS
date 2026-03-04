## MODIFIED Requirements

### Requirement: Tenant data isolation filterability
The system SHALL expose tenant ownership through a shared tenant provider contract on all tenant-scoped entities so repositories and query services can enforce company isolation by `CompanyId` consistently.

#### Scenario: Repository query scoping
- **WHEN** querying tenant-scoped entities in repositories
- **THEN** the entity model provides `CompanyId` through the tenant provider contract for applying tenant filters

#### Scenario: Contract-based tenant filtering
- **WHEN** data-access logic receives an entity type that implements the tenant provider contract
- **THEN** tenant scoping can be applied without relying on property-name conventions or reflection
