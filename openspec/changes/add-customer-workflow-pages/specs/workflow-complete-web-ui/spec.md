## MODIFIED Requirements

### Requirement: UI SHALL cover all required business workflows from configuration context
The WebApp SHALL expose continuity-critical workflows during the UI rework preparation phase, including public landing entry, self-service company signup onboarding, authentication/login lifecycle, customer/company post-auth entry, company user administration workflow for authorized company operators, and complete customer self-service workflows for subscription discovery, subscription management, preferences/exclusions, deliveries, and complaints.

#### Scenario: Continuity workflow remains reachable
- **WHEN** a workflow belongs to the continuity-critical scope for rework preparation
- **THEN** users can navigate to at least one corresponding UI flow that starts and completes that workflow

#### Scenario: Non-continuity workflow is absent during cleanup phase
- **WHEN** a workflow is outside the continuity-critical scope
- **THEN** the workflow is not required to be reachable in `WebApp` during the cleanup phase

#### Scenario: Customer self-service workflow set is reachable
- **WHEN** a signed-in customer accesses the customer view shell
- **THEN** the UI provides reachable pages for discover subscriptions, manage subscriptions, preferences/exclusions, deliveries, and complaints

### Requirement: Customer and company view separation SHALL be explicit
For retained continuity flows, the UI MUST preserve explicit separation between customer and company contexts and must maintain authorization boundaries for single-account users who can operate in both contexts.

#### Scenario: User switches context between retained customer and company continuity flows
- **WHEN** a user with both customer and company roles navigates retained continuity flows
- **THEN** the UI clearly distinguishes customer-view context from company-view context
- **AND** authorization boundaries remain enforced

### Requirement: Company user administration workflow is available in company view
The WebApp SHALL include a company-scoped **Manage Users and Roles** workflow in company view that is reachable for CompanyOwner and CompanyAdmin and hidden or inaccessible for other company roles.

#### Scenario: Authorized role sees management workflow entry
- **WHEN** a signed-in CompanyOwner or CompanyAdmin accesses the company operations shell
- **THEN** the UI shows navigation to the Manage Users and Roles page

#### Scenario: Unauthorized role cannot access management page
- **WHEN** a signed-in CompanyManager or CompanyEmployee attempts to access the Manage Users and Roles route
- **THEN** the UI and backend authorization deny access

### Requirement: Manage Users and Roles page supports add, role-change, remove, and ownership-transfer actions
The WebApp SHALL provide a single management page that supports adding existing users by email, changing member roles, removing members, and ownership transfer according to role permissions and guardrails.

#### Scenario: Add existing user by email from management page
- **WHEN** an authorized actor submits an existing email and selected role on the management page
- **THEN** the user appears in the company membership list with the selected role

#### Scenario: Update role from membership list
- **WHEN** an authorized actor updates a member role from the management page
- **THEN** the UI reflects the new persisted role

#### Scenario: Remove member from membership list
- **WHEN** an authorized actor removes a non-owner member from the management page
- **THEN** the member is no longer listed for the active company

#### Scenario: Owner performs ownership transfer from management page
- **WHEN** the current CompanyOwner initiates ownership transfer to an eligible member
- **THEN** the page reflects the new owner and updated prior-owner role after successful completion

