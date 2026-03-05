## Purpose

Define requirements for self-service tenant onboarding that creates a new company tenant and initial company owner account safely.

## Requirements

### Requirement: Self-service signup SHALL create a new tenant and initial owner account
The system SHALL provide a signup flow that creates a company tenant and provisions an initial user with the `CompanyOwner` role as part of onboarding.

#### Scenario: New company onboarding completes successfully
- **WHEN** an anonymous user submits valid self-service onboarding data
- **THEN** the system creates a new company tenant record
- **AND** the system creates the user account and assigns `CompanyOwner` in the new tenant context

### Requirement: Tenant onboarding SHALL enforce uniqueness and validity constraints
The onboarding flow MUST validate required fields and reject signup attempts that violate uniqueness or identity constraints.

#### Scenario: Duplicate onboarding identity is submitted
- **WHEN** a user attempts onboarding with an email that is not valid for creating a new owner association for the requested tenant setup
- **THEN** the system rejects the request with actionable validation errors
- **AND** no partially created tenant setup is exposed as successful completion

### Requirement: Onboarding failures SHALL not leave unsafe cross-tenant associations
If onboarding fails after partial progress, the system MUST prevent invalid cross-tenant linkage and preserve tenant data isolation guarantees.

#### Scenario: Role assignment fails after tenant creation step
- **WHEN** tenant creation succeeds but owner role assignment fails
- **THEN** the system records the onboarding attempt as incomplete and prevents authenticated access as a tenant owner until resolved
- **AND** the failed onboarding state does not grant access to other tenant data
