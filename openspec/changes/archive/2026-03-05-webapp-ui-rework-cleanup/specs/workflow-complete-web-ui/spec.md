## MODIFIED Requirements

### Requirement: UI SHALL cover all required business workflows from configuration context
The WebApp SHALL expose only continuity-critical workflows during the UI rework preparation phase: public landing entry, self-service company signup onboarding, authentication/login lifecycle, and minimal customer/company entry flows required immediately post-authentication. Workflows outside this continuity scope are not required in this phase and MAY be removed.

#### Scenario: Continuity workflow remains reachable
- **WHEN** a workflow belongs to the continuity-critical scope for rework preparation
- **THEN** users can navigate to at least one corresponding UI flow that starts and completes that workflow

#### Scenario: Non-continuity workflow is absent during cleanup phase
- **WHEN** a workflow is outside the continuity-critical scope
- **THEN** the workflow is not required to be reachable in `WebApp` during the cleanup phase

### Requirement: Customer and company view separation SHALL be explicit
For retained continuity flows, the UI MUST preserve explicit separation between customer and company contexts and must maintain authorization boundaries for single-account users who can operate in both contexts.

#### Scenario: User switches context between retained customer and company continuity flows
- **WHEN** a user with both customer and company roles navigates retained continuity flows
- **THEN** the UI clearly distinguishes customer-view context from company-view context
- **AND** authorization boundaries remain enforced

