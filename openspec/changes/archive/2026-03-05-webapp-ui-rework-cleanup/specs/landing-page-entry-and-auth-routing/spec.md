## MODIFIED Requirements

### Requirement: Public landing page SHALL be the default application entry
The system SHALL keep a public landing page at the root route as the primary continuity entry point during cleanup, presenting clear calls to action for company signup and existing-user login.

#### Scenario: Anonymous user opens root route during cleanup phase
- **WHEN** an unauthenticated user navigates to `/`
- **THEN** the system renders landing content with visible register and login actions

### Requirement: Landing page SHALL route users to correct authentication flows
The landing page MUST continue to route users directly to supported registration and login endpoints that remain in scope for rework continuity, without dependency on removed legacy pages.

#### Scenario: User selects self-service signup from landing page after cleanup
- **WHEN** a user activates the signup action on the landing page
- **THEN** the system navigates the user to the retained tenant onboarding registration flow

#### Scenario: User selects login from landing page after cleanup
- **WHEN** a user activates the login action on the landing page
- **THEN** the system navigates the user to the retained existing-user login flow

### Requirement: Post-authentication routing SHALL resolve to an allowed context
After successful login, the system SHALL redirect users only to authorized retained slug-scoped entry destinations and MUST NOT redirect to removed legacy workflow pages.

#### Scenario: Returning user logs in successfully after cleanup
- **WHEN** an existing user completes login
- **THEN** the system redirects the user to an authorized retained slug-scoped context entry page
- **AND** the redirected URL includes an authorized slug segment
- **AND** the system does not expose unauthorized tenant data

