## MODIFIED Requirements

### Requirement: Post-authentication routing SHALL resolve to an allowed context
After successful login, the system SHALL route the authenticated user to an authorized slug-scoped company or customer operations entry URL and MUST NOT route to unauthorized tenant data.

#### Scenario: Returning user logs in successfully
- **WHEN** an existing user completes login
- **THEN** the system redirects the user to an authorized slug-scoped application context entry page
- **AND** the redirected URL includes the selected authorized slug segment
- **AND** the system does not expose data from tenants the user is not assigned to
