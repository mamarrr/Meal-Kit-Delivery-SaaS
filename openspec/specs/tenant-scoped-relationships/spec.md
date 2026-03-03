## ADDED Requirements

### Requirement: Company ownership required
The system SHALL associate every tenant-scoped entity with a `CompanyId` foreign key matching the schema.

#### Scenario: Company FK present on tenant entities
- **WHEN** defining entities such as Customer, Recipe, DeliveryZone, WeeklyMenu, Box, or PlatformSubscription
- **THEN** each entity includes a required `CompanyId` property and `Company` navigation

### Requirement: Tenant data isolation filterability
The system SHALL expose `CompanyId` on all tenant-scoped entities to support query filtering by tenant.

#### Scenario: Repository query scoping
- **WHEN** querying tenant-scoped entities in repositories
- **THEN** the entity model provides `CompanyId` for applying tenant filters

### Requirement: Join tables retain tenant scope
The system SHALL include `CompanyId` on join entities where the schema defines it (e.g., BoxPrice).

#### Scenario: Join entity tenant ownership
- **WHEN** mapping join entities with a `Company_ID_FK` column
- **THEN** the corresponding entity includes `CompanyId` and a `Company` navigation
