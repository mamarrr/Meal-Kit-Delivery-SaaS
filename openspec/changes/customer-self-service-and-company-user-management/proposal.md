## Why

The platform currently assumes company-centric onboarding and management flows, which blocks direct customer self-service registration and creates ambiguity between customer and company user experiences. This change is needed now to unlock customer acquisition, establish a first-class customer subscription lifecycle, and align company user administration with required role governance.

## What Changes

- Allow visitors to register from the top-right authentication entry point without creating a company.
- Introduce a dedicated customer view after login where users can see and manage their MealSubscriptions.
- Add a customer discovery flow to browse available MealSubscriptions and subscribe to new ones.
- Rework company user management so CompanyOwner and CompanyAdmin can invite users by email and assign company roles (Admin, Manager, Employee).
- Add ownership transfer capability restricted to CompanyOwner.
- Update navigation and post-auth routing so users with mixed context can access customer and company experiences appropriately.

## Capabilities

### New Capabilities
- `customer-subscription-self-service`: Customer-facing browsing, subscription enrollment, and personal MealSubscription management flows.
- `company-user-invitation-and-role-assignment`: Tenant-scoped invitation flow with role assignment (Admin, Manager, Employee) by CompanyOwner/CompanyAdmin.
- `company-ownership-transfer`: Secure ownership transfer workflow restricted to CompanyOwner with auditability expectations.

### Modified Capabilities
- `landing-page-entry-and-auth-routing`: Expand top-right auth entry and post-login routing to support non-company registration and customer-first landing behavior.
- `standard-app-shell-layout`: Add/adjust shell navigation to clearly separate customer and company contexts for authenticated users.
- `self-service-tenant-onboarding`: Clarify coexistence of company onboarding with independent customer account registration.

## Impact

- Web UI and routing: landing page auth affordances, account registration path, customer pages, company user management pages, and shell navigation.
- Authorization and policies: role checks for invitation/assignment and ownership transfer constraints.
- Application/domain layers: customer subscription query/command flows, invitation/role assignment services, ownership transfer operations.
- Data and audit surfaces: invitation records/tokens (if modeled), role change records, ownership transfer audit trail.
- Testing: integration/authorization coverage for customer registration, subscription self-service, invitation/role management, and ownership transfer guardrails.
