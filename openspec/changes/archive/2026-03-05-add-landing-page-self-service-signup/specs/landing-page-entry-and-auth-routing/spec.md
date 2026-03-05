## ADDED Requirements

### Requirement: Public landing page SHALL be the default application entry
The system SHALL expose a public landing page at the root route that presents clear primary actions for self-service company signup and existing-user login.

#### Scenario: Anonymous user opens root route
- **WHEN** an unauthenticated user navigates to `/`
- **THEN** the system renders the landing page with visible calls to action for register and login

### Requirement: Landing page SHALL route users to correct authentication flows
The landing page MUST provide direct navigation to the registration and login endpoints without requiring users to discover links from internal pages.

#### Scenario: User selects self-service signup from landing page
- **WHEN** a user activates the signup action on the landing page
- **THEN** the system navigates the user to the tenant onboarding registration flow

#### Scenario: User selects login from landing page
- **WHEN** a user activates the login action on the landing page
- **THEN** the system navigates the user to the existing user login flow

### Requirement: Post-authentication routing SHALL resolve to an allowed context
After successful login, the system SHALL route the authenticated user to an allowed company or customer context entry point and MUST NOT route to unauthorized tenant data.

#### Scenario: Returning user logs in successfully
- **WHEN** an existing user completes login
- **THEN** the system redirects the user to an authorized application context entry page
- **AND** the system does not expose data from tenants the user is not assigned to
