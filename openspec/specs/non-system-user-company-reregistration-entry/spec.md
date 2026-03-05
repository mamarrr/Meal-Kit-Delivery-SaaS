## Purpose

Define eligibility and navigation behavior for authenticated non-system-role users to start registration of an additional company from the shared shell.

## Requirements

### Requirement: Eligible authenticated users SHALL see a register-new-company sidebar action
The system SHALL display a "Register new company" action in the authenticated left navigation sidebar bottom section for users who are authenticated and are not assigned any system-level role.

#### Scenario: Non-system user with at least one company loads shell page
- **WHEN** an authenticated user without system roles renders a shell-based page
- **THEN** the sidebar shows a "Register new company" action in the bottom section
- **AND** the action remains visible even when the user already has existing company associations

#### Scenario: System-role user loads shell page
- **WHEN** an authenticated user with any system-level role renders a shell-based page
- **THEN** the sidebar does not show the "Register new company" action

### Requirement: Register-new-company sidebar action SHALL route to tenant onboarding entry
The system SHALL route the "Register new company" sidebar action to the tenant onboarding entry path used for company self-service registration.

#### Scenario: Eligible user activates register-new-company action
- **WHEN** an eligible authenticated user selects "Register new company" from the sidebar
- **THEN** the user is navigated to the tenant onboarding entry flow
- **AND** the onboarding flow starts a new company registration attempt independent of existing company memberships
