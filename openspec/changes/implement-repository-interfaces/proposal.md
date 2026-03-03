## Why

The data access layer lacks a consistent repository contract and concrete implementations for domain entities, which makes database operations uneven and harder to test or enforce tenant isolation. Introducing a standard repository interface/implementation set aligns the DAL with the project architecture now, before higher-level features depend on it.

## What Changes

- Define repository interfaces per domain aggregate in the DAL contracts layer, using shared base repository abstractions.
- Implement EF Core repositories for each interface in the DAL implementation layer, inheriting from the base EF repository.
- Establish a consistent pattern for repository registration and usage across BLL/DAL boundaries.

## Capabilities

### New Capabilities
- `repository-contracts`: Repository interface requirements for domain aggregates, including tenant-aware querying expectations.
- `repository-implementations`: EF Core repository implementation requirements conforming to the base repository pattern.

### Modified Capabilities
- (none)

## Impact

- `Base.Contracts.DAL` and `Base.DAL.EF` for shared abstractions and base repository usage.
- `App.DAL.EF` for concrete repository implementations.
- `App.BLL` and `WebApp` wiring/usage of repositories where data access is coordinated.
- Documentation/specs under `openspec/specs` for repository contract and implementation behavior.
