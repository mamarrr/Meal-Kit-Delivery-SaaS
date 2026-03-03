## 1. Domain Contract and Entity Coverage

- [ ] 1.1 Create `ITenantProvider` in the domain layer with required `CompanyId` contract.
- [ ] 1.2 Identify all company-owned domain entities and implement `ITenantProvider` on each applicable type.
- [ ] 1.3 Confirm platform/global entities are not incorrectly marked with `ITenantProvider`.

## 2. Data Access Tenant Enforcement

- [ ] 2.1 Update tenant-scoped repository/query paths to enforce filtering by current tenant `CompanyId` for `ITenantProvider` entities.
- [ ] 2.2 Ensure create/update flows preserve tenant ownership consistency (`CompanyId` cannot drift across tenants).
- [ ] 2.3 Verify no-tracking update paths still explicitly call `Update()` where modifications occur.

## 3. Validation and Regression Safety

- [ ] 3.1 Add/adjust tests (or equivalent verification checks) proving tenant A cannot access tenant B data for contract-implementing entities.
- [ ] 3.2 Add/adjust tests confirming non-tenant/global entities are unaffected by tenant provider requirements.
- [ ] 3.3 Run build/test validation and resolve any contract adoption regressions.

## 4. Documentation and Implementation Readiness

- [ ] 4.1 Document tenant-scoped entity conventions for future development (when to implement `ITenantProvider`).
- [ ] 4.2 Cross-check implementation against new and modified specs for completeness.
- [ ] 4.3 Prepare change for apply phase handoff.