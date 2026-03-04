## 1. BLL contracts structure

- [ ] 1.1 Create `App.Contracts.BLL` project (if missing) and add to solution
- [ ] 1.2 Define base service abstractions (interfaces and common patterns) for async CRUD and tenant context
- [ ] 1.3 Add service interface folders by functional area (Core, Menu, Delivery, Subscription, Identity)
- [ ] 1.4 Create service interfaces for each aggregate root with async CRUD signatures

## 2. BLL service implementations

- [ ] 2.1 Add service implementation folders mirroring contract areas in `App.BLL`
- [ ] 2.2 Implement services that wrap repository interfaces and enforce `CompanyId` scoping
- [ ] 2.3 Ensure update flows use explicit update/attach semantics for no-tracking EF
- [ ] 2.4 Add any domain-specific operations required by services (beyond CRUD)

## 3. Dependency injection and wiring

- [ ] 3.1 Add DI registration extension in `App.BLL` for all services
- [ ] 3.2 Register `App.Contracts.BLL` and `App.BLL` in `WebApp` startup
- [ ] 3.3 Update controllers/pages to depend on BLL service interfaces instead of repositories

## 4. Validation and alignment

- [ ] 4.1 Verify tenant scoping is consistently enforced across services
- [ ] 4.2 Validate service naming conventions and folder structure are consistent
- [ ] 4.3 Run build/tests to ensure service layer compiles and resolves via DI