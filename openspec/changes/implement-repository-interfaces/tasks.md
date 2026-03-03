## 1. Repository Contracts

- [x] 1.1 Inventory domain aggregates and map to repository interfaces in `App.Contracts.DAL`
- [x] 1.2 Add repository interface definitions per aggregate inheriting base DAL contract
- [x] 1.3 Define tenant-aware query methods/parameters for tenant-scoped aggregates
- [x] 1.4 Document no-tracking update expectations in repository interfaces

## 2. Repository Implementations

- [x] 2.1 Create EF Core repository classes per interface in `App.DAL.EF`, inheriting `BaseRepository`
- [x] 2.2 Implement tenant-aware filters in repository query methods
- [x] 2.3 Ensure update operations call EF Core `Update()` for existing entities

## 3. Wiring and Validation

- [x] 3.1 Register repositories in DI composition root (DAL/BLL setup extensions)
- [ ] 3.2 Update BLL/data access usage to depend on repository interfaces
- [ ] 3.3 Add/adjust tests or verification steps for tenant filtering and update behavior
