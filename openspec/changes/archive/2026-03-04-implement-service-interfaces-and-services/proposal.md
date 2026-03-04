## Why

The project has repository interfaces and EF implementations, but the service layer contracts and implementations are missing, leaving business logic without a structured abstraction between WebApp and data access. Implementing service interfaces and services now enables consistent BLL orchestration, validation, and unit-testable business rules aligned with the layered architecture.

## What Changes

- Introduce BLL service interfaces to define business operations over domain entities and aggregates.
- Implement BLL service classes that orchestrate repository calls, enforce tenant scoping, and apply domain rules.
- Wire service registrations for dependency injection to connect WebApp → App.BLL → App.DAL.EF.
- Standardize service patterns (naming, base classes, async CRUD, tenant-aware access) across functional areas.

## Capabilities

### New Capabilities
- `service-contracts`: Defines the required BLL service interfaces for core, menu, delivery, subscription, and identity operations.
- `service-implementations`: Implements the BLL services that fulfill the contracts and connect to repository interfaces.

### Modified Capabilities
<!-- Existing capabilities whose REQUIREMENTS are changing (not just implementation).
     Only list here if spec-level behavior changes. Each needs a delta spec file.
     Use existing spec names from openspec/specs/. Leave empty if no requirement changes. -->

## Impact

- App.BLL: add service interfaces, implementations, and DI registration.
- App.Contracts.BLL (if present/added): new contracts for service layer.
- WebApp: update service wiring and usage patterns.
- Cross-cutting: alignment with tenant isolation, no-tracking EF update patterns, and async CRUD operations.
