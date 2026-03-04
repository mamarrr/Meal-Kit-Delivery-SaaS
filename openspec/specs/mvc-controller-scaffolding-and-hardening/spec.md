## Purpose

Define requirements for generating MVC controller baselines and hardening them to meet architecture, tenant-safety, no-tracking update, and view-model standards in WebApp.

## Requirements

### Requirement: Controllers SHALL be scaffolded before manual hardening
The system SHALL generate initial MVC controllers for relevant WebApp entities by using `dotnet aspnet-codegenerator` from the WebApp project, then apply manual hardening to meet project conventions.

#### Scenario: Controller scaffold baseline is created
- **WHEN** a new domain workflow is exposed in WebApp
- **THEN** an MVC controller baseline is generated with `dotnet aspnet-codegenerator controller` before custom edits are applied

### Requirement: Hardened controllers SHALL enforce architecture boundaries
Controllers SHALL depend on BLL service contracts and SHALL NOT call repository implementations directly.

#### Scenario: Business operations go through BLL contracts
- **WHEN** a controller action performs create, read, update, or delete business logic
- **THEN** the action uses injected service interfaces from App.Contracts.BLL and does not reference DAL repositories

### Requirement: Hardened controllers SHALL preserve tenant-safe behavior
Controllers handling tenant-owned data MUST require tenant context and MUST pass tenant scope to service operations for all tenant-scoped queries and commands.

#### Scenario: Tenant context is applied on every tenant action
- **WHEN** a user accesses a tenant-owned entity action
- **THEN** the controller enforces current tenant context and only forwards tenant-scoped operations to services

### Requirement: Update actions SHALL be no-tracking compatible end-to-end
Controller update flows MUST call service paths that use explicit update semantics compatible with EF Core no-tracking behavior.

#### Scenario: Edit action persists detached updates
- **WHEN** a user submits an edit form for an existing entity
- **THEN** the controller routes the update through a service/repository path that performs explicit update semantics so changes are persisted

### Requirement: Controllers SHALL use view models for complex views
Controllers MUST provide dedicated view models for create/edit/filter/index screens requiring extra UI state (such as select lists, paging, filters, and workflow status) and MUST NOT use ViewBag or ViewData.

#### Scenario: Create/Edit uses typed view model
- **WHEN** a controller renders a view requiring more than a single entity payload
- **THEN** it supplies a strongly typed view model containing all required UI data without ViewBag or ViewData
