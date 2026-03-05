## MODIFIED Requirements

### Requirement: Public landing page SHALL be the default application entry
The system SHALL expose a public landing page at the root route that presents clear primary actions for customer registration, self-service company signup, and existing-user login.

#### Scenario: Anonymous user opens root route
- **WHEN** an unauthenticated user navigates to `/`
- **THEN** the system renders the landing page with visible calls to action for customer register, company signup, and login

### Requirement: Landing page SHALL route users to correct authentication flows
The landing page MUST provide direct navigation to customer registration, tenant onboarding registration, and login endpoints without requiring users to discover links from internal pages.

#### Scenario: User selects customer registration from landing page
- **WHEN** a user activates the customer registration action on the landing page
- **THEN** the system navigates the user to account registration that does not require company creation

#### Scenario: User selects self-service signup from landing page
- **WHEN** a user activates the company signup action on the landing page
- **THEN** the system navigates the user to the tenant onboarding registration flow

#### Scenario: User selects login from landing page
- **WHEN** a user activates the login action on the landing page
- **THEN** the system navigates the user to the existing user login flow

### Requirement: Post-authentication routing SHALL resolve to an allowed context
After successful login, the system SHALL route the authenticated user to an authorized customer or slug-scoped company operations entry URL and MUST NOT route to unauthorized tenant data.

#### Scenario: Customer-only user logs in successfully
- **WHEN** an existing user with no active company memberships completes login
- **THEN** the system redirects the user to the customer operations entry page
- **AND** the redirected destination does not require tenant onboarding completion

#### Scenario: Company user logs in successfully
- **WHEN** an existing user with authorized company memberships completes login
- **THEN** the system redirects the user to an authorized slug-scoped application context entry page
- **AND** the redirected URL includes the selected authorized slug segment
- **AND** the system does not expose data from tenants the user is not assigned to

