## 1. Authentication Entry and Context Routing

- [x] 1.1 Update top-right anonymous account actions to expose separate customer registration and company signup links.
- [x] 1.2 Implement customer registration flow that creates an account without creating a company tenant.
- [x] 1.3 Update post-login routing to resolve authorized destination between customer context and slug-scoped company context.
- [x] 1.4 Add context-selection/remembered-context behavior for users associated with both customer and company roles.

## 2. Customer MealSubscription Self-Service

- [x] 2.1 Add customer-facing controller actions and view models for “My MealSubscriptions” and “Browse MealSubscriptions”.
- [x] 2.2 Implement BLL/DAL query methods to list customer-owned subscriptions and tenant-scoped subscribable offers.
- [x] 2.3 Implement subscription enrollment command flow that links selected offers to the authenticated customer account.
- [x] 2.4 Add authorization checks ensuring customers only access their own subscription data and allowed tenant offers.

## 3. Company User Invitation and Role Assignment Rework

- [x] 3.1 Rework company user management UI to support invite-by-email and role selection (Admin, Manager, Employee).
- [x] 3.2 Implement invitation creation and acceptance/linking service flow with tenant-scoped validation.
- [x] 3.3 Implement role assignment/update operations restricted to CompanyOwner and CompanyAdmin.
- [x] 3.4 Add safeguards and validations preventing unauthorized invitation or role mutation attempts.

## 4. Ownership Transfer Workflow

- [x] 4.1 Add ownership transfer UI/action available only to CompanyOwner in company user management.
- [x] 4.2 Implement service-layer ownership transfer operation requiring eligible active tenant member target.
- [x] 4.3 Ensure transfer is atomic: promote new owner, demote previous owner, and persist all updates with explicit `Update()` calls.
- [ ] 4.4 Add audit trail persistence for ownership transfer actor, target, tenant, and timestamp.

## 5. Navigation, Authorization, and Data Isolation Hardening

- [x] 5.1 Update shared shell/sidebar composition to render customer vs company navigation based on active context.
- [ ] 5.2 Add/adjust authorization policies for invitation management, role assignment, and ownership transfer.
- [x] 5.3 Verify tenant isolation in all new/modified repositories and services by enforcing CompanyId-scoped queries.
- [x] 5.4 Validate no-tracking update flows use explicit update operations before save in all mutable workflows.

## 6. Verification and Regression Coverage

- [ ] 6.1 Add integration tests for customer registration without company creation and customer post-login routing.
- [ ] 6.2 Add integration tests for customer browse-and-subscribe flow and My MealSubscriptions visibility constraints.
- [ ] 6.3 Add integration tests for company invite/role assignment authorization boundaries.
- [ ] 6.4 Add integration tests for ownership transfer success, validation failures, and unauthorized attempts.
- [ ] 6.5 Run full test suite and fix regressions in existing landing, shell layout, and onboarding behavior.
