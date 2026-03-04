## ADDED Requirements

### Requirement: Company ownership required
The system SHALL associate every tenant-scoped entity with a `CompanyId` foreign key matching the schema.

#### Scenario: Company FK present on tenant entities
- **WHEN** defining entities such as Customer, Recipe, DeliveryZone, WeeklyMenu, Box, or PlatformSubscription
- **THEN** each entity includes a required `CompanyId` property and `Company` navigation

### Requirement: Tenant data isolation filterability
The system SHALL expose tenant ownership through a shared tenant provider contract on all tenant-scoped entities so repositories and query services can enforce company isolation by `CompanyId` consistently.

#### Scenario: Repository query scoping
- **WHEN** querying tenant-scoped entities in repositories
- **THEN** the entity model provides `CompanyId` through the tenant provider contract for applying tenant filters

#### Scenario: Contract-based tenant filtering
- **WHEN** data-access logic receives an entity type that implements the tenant provider contract
- **THEN** tenant scoping can be applied without relying on property-name conventions or reflection

### Requirement: Join tables retain tenant scope
The system SHALL include `CompanyId` on join entities where the schema defines it (e.g., BoxPrice).

#### Scenario: Join entity tenant ownership
- **WHEN** mapping join entities with a `Company_ID_FK` column
- **THEN** the corresponding entity includes `CompanyId` and a `Company` navigation
