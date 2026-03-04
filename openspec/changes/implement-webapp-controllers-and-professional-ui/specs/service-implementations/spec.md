## MODIFIED Requirements

### Requirement: Services registered for DI and consumed by WebApp
BLL services MUST be registered with dependency injection and consumed by the WebApp through BLL interfaces, including newly scaffolded-and-hardened controllers that expose required customer/company/system workflows.

#### Scenario: Services available via DI
- **WHEN** the WebApp resolves a BLL service from the service container
- **THEN** the concrete implementation is provided via its interface

#### Scenario: New workflow controller resolves required service dependencies
- **WHEN** a controller for a newly surfaced workflow is activated
- **THEN** all required BLL service dependencies resolve successfully through DI without direct repository access

