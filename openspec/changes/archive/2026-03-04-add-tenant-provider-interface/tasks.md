## 1. Domain Contract and Entity Coverage

- [x] 1.1 Create `ITenantProvider` in the domain layer with required `CompanyId` contract.
- [x] 1.2 Identify all company-owned domain entities and implement `ITenantProvider` on each applicable type.
- [x] 1.3 Confirm platform/global entities are not incorrectly marked with `ITenantProvider`.

## 2. Data Access Tenant Enforcement

- [x] 2.1 Update tenant-scoped repository/query paths to enforce filtering by current tenant `CompanyId` for `ITenantProvider` entities.
- [x] 2.2 Ensure create/update flows preserve tenant ownership consistency (`CompanyId` cannot drift across tenants).
- [x] 2.3 Verify no-tracking update paths still explicitly call `Update()` where modifications occur.

## 3. Validation and Regression Safety

- [x] 3.1 Add/adjust tests (or equivalent verification checks) proving tenant A cannot access tenant B data for contract-implementing entities.
- [x] 3.2 Add/adjust tests confirming non-tenant/global entities are unaffected by tenant provider requirements.
- [x] 3.3 Run build/test validation and resolve any contract adoption regressions.

## 4. Documentation and Implementation Readiness

- [x] 4.1 Document tenant-scoped entity conventions for future development (when to implement `ITenantProvider`).
- [x] 4.2 Cross-check implementation against new and modified specs for completeness.
- [x] 4.3 Prepare change for apply phase handoff.
