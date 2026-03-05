## MODIFIED Requirements

### Requirement: Self-service signup SHALL create a new tenant and initial owner account
The system SHALL provide a signup flow that creates a company tenant and provisions an initial user with the `CompanyOwner` role as part of onboarding. The flow MUST support initiation by eligible authenticated users who are not assigned any system-level role, including users who already have one or more existing company associations.

#### Scenario: New company onboarding completes successfully
- **WHEN** an anonymous user submits valid self-service onboarding data
- **THEN** the system creates a new company tenant record
- **AND** the system creates the user account and assigns `CompanyOwner` in the new tenant context

#### Scenario: Eligible authenticated user starts additional company onboarding
- **WHEN** an authenticated user without any system-level role initiates tenant onboarding and submits valid data
- **THEN** the system creates a new company tenant record
- **AND** the system associates the existing user account as `CompanyOwner` in the new tenant context
- **AND** existing company memberships for that user remain intact

