## 1. Repository Contracts

- [ ] 1.1 Inventory domain aggregates and map to repository interfaces in `App.DAL.Contracts`
- [ ] 1.2 Add repository interface definitions per aggregate inheriting base DAL contract
- [ ] 1.3 Define tenant-aware query methods/parameters for tenant-scoped aggregates
- [ ] 1.4 Document no-tracking update expectations in repository interfaces

## 2. Repository Implementations

- [ ] 2.1 Create EF Core repository classes per interface in `App.DAL.EF`, inheriting `BaseRepository`
- [ ] 2.2 Implement tenant-aware filters in repository query methods
- [ ] 2.3 Ensure update operations call EF Core `Update()` for existing entities

## 3. Wiring and Validation

- [ ] 3.1 Register repositories in DI composition root (DAL/BLL setup extensions)
- [ ] 3.2 Update BLL/data access usage to depend on repository interfaces
- [ ] 3.3 Add/adjust tests or verification steps for tenant filtering and update behavior
