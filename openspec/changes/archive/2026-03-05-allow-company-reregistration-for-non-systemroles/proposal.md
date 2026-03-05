## Why

Logged-in users who are not assigned any system-level roles currently lack a clear, persistent entry point to register a company from inside the authenticated app shell. This slows tenant onboarding and blocks valid multi-company scenarios where the same user should be able to start another company even if they already belong to one.

## What Changes

- Add an authenticated navigation affordance in the left sidebar (bottom section) for non-system-role users to start company registration.
- Ensure the option remains visible for eligible users regardless of whether they already have one or more companies.
- Define visibility rules so system-role users do not see this onboarding action in the company/customer app shell.
- Define interaction expectations for routing from the sidebar action into the existing company onboarding/registration flow.

## Capabilities

### New Capabilities
- `non-system-user-company-reregistration-entry`: Adds a persistent sidebar action that allows eligible logged-in users to initiate registration of a new company at any time.

### Modified Capabilities
- `standard-app-shell-layout`: Expands authenticated navigation requirements to include a bottom-positioned "register new company" action with role-based visibility rules.
- `self-service-tenant-onboarding`: Clarifies that already-associated users may initiate additional company onboarding attempts when they are not in system roles.

## Impact

- Affected UI: authenticated left navigation/sidebar layout and navigation rendering logic.
- Affected authorization/context logic: role checks for system-level roles versus non-system users.
- Affected onboarding entry routing: integration point from app shell navigation into tenant registration flow.
- Affected tests/spec coverage: UI visibility and eligibility behavior for users with existing company associations.
