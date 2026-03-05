## Purpose

Define explicit user-controlled context and company selection behavior in the authenticated shell, including authorization constraints and persisted active selection validation.

## Requirements

### Requirement: Context selector in authenticated shell
The system SHALL present a context selector in the authenticated topbar near account greeting and logout actions.

#### Scenario: Available contexts are role constrained
- **WHEN** an authenticated user opens any page rendered with the shared authenticated layout
- **THEN** the UI shows context options for `customer`, `company`, and `system`
- **AND** `system` is enabled only if the user has at least one of `SystemAdmin`, `SystemSupport`, or `SystemBilling`
- **AND** unavailable contexts are not selectable

### Requirement: Company selector for multi-membership users
The system SHALL allow explicit active-company selection for users with multiple active company memberships.

#### Scenario: Multi-company member sees dropdown
- **GIVEN** a signed-in user has more than one active `CompanyAppUser` membership
- **WHEN** the authenticated shell is rendered
- **THEN** a company dropdown is displayed in topbar controls
- **AND** each option maps to a company the user is actively assigned to

#### Scenario: Single-company member does not need dropdown
- **GIVEN** a signed-in user has exactly one active company membership
- **WHEN** the authenticated shell is rendered
- **THEN** no multi-select company dropdown is required

### Requirement: Active selection persistence and validation
The system SHALL persist active context and active company selections and validate them server-side on every switch request.

#### Scenario: Unauthorized selection is rejected
- **WHEN** a user submits a context not allowed by role, or a company not in active membership
- **THEN** the system rejects the selection
- **AND** does not persist unauthorized values
- **AND** falls back to a valid default selection
