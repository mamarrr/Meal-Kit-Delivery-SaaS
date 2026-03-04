# Tenant Provider Contract

## Purpose
Define the tenant provider contract for company-owned entities. (TBD: expand scope and constraints.)

## Requirements

### Requirement: Tenant-scoped entities implement tenant provider contract
The system SHALL define a tenant provider contract and require every company-owned domain entity to implement it so tenant ownership is explicit and discoverable.

#### Scenario: Company-owned entity is marked as tenant-scoped
- **WHEN** a domain entity stores business data that belongs to a single company tenant
- **THEN** the entity MUST implement the tenant provider contract

### Requirement: Tenant provider exposes CompanyId
The tenant provider contract SHALL expose a non-null tenant discriminator property named `CompanyId` that represents the owning company.

#### Scenario: Repository code reads tenant discriminator
- **WHEN** repository or query logic handles an entity implementing the tenant provider contract
- **THEN** it can read `CompanyId` from the contract without relying on reflection or naming conventions

### Requirement: Global entities remain outside tenant provider contract
The system MUST NOT require platform-global entities to implement the tenant provider contract.

#### Scenario: Entity is system-level and not company-owned
- **WHEN** a domain entity represents cross-tenant or platform-level data
- **THEN** it does not implement the tenant provider contract
