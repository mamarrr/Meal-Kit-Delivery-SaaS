## 1. Service Contracts and Business Rules

- [x] 1.1 Extend company/user service contracts to support add-by-email membership creation, role changes, membership removal, and ownership transfer operations.
- [x] 1.2 Implement tenant-scoped validation and authorization rules in BLL for add-by-email, role mutation, removal, and ownership transfer.
- [x] 1.3 Implement ownership transfer as a consistent state transition that preserves a single valid company owner.
- [x] 1.4 Ensure all update paths use explicit update semantics compatible with EF Core no-tracking behavior.

## 2. Data Access and Tenant-Scoped Query Support

- [x] 2.1 Add or extend DAL repository methods for resolving existing users by normalized email and loading company membership/role state.
- [x] 2.2 Implement repository updates required for membership add/remove and role/owner transitions with strict CompanyId scoping.
- [x] 2.3 Add data-layer guards/tests to prevent cross-tenant membership mutation.

## 3. WebApp Manage Users and Roles Experience

- [x] 3.1 Add company-scoped controller actions and ViewModels for listing members and handling add, role update, remove, and ownership transfer posts.
- [x] 3.2 Create the Manage Users and Roles Razor view with forms and validation messages for all supported actions.
- [x] 3.3 Wire the page into company operations navigation and restrict visibility/access to CompanyOwner and CompanyAdmin.
- [x] 3.4 Enforce owner-only ownership transfer action path in UI and server-side endpoints.

## 4. Verification and Regression Coverage

- [x] 4.1 Add/extend unit tests for BLL authorization matrix and invariants (email exists check, duplicate prevention, owner protections).
- [x] 4.2 Add/extend integration tests for tenant isolation and end-to-end role/membership workflows.
- [x] 4.3 Validate routing/navigation behavior and forbidden access responses for unauthorized roles.
- [x] 4.4 Run full test suite and fix regressions introduced by the change.
