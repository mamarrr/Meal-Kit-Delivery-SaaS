## ADDED Requirements

### Requirement: Domain entity coverage
The system SHALL define a domain entity for every table in `openspec/schema.sql`.

#### Scenario: Entity list completeness
- **WHEN** the domain model is reviewed against the schema table list
- **THEN** each table has a corresponding entity class in `App.Domain`

### Requirement: Required fields mapping
The system SHALL map all NOT NULL columns to non-nullable properties in the corresponding domain entities.

#### Scenario: Required column enforcement
- **WHEN** creating an entity instance for a table with NOT NULL columns
- **THEN** the domain entity exposes non-nullable properties for those columns

### Requirement: Audit and soft delete columns
The system SHALL include `CreatedAt`, `UpdatedAt`, and `DeletedAt` properties on entities where those columns exist in the schema.

#### Scenario: Soft delete properties included
- **WHEN** an entity table contains a `DeletedAt` column
- **THEN** the domain entity exposes a nullable `DeletedAt` property
