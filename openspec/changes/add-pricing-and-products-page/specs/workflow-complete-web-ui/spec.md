## MODIFIED Requirements

### Requirement: UI SHALL cover all required business workflows from configuration context
The WebApp SHALL expose continuity-critical workflows during the UI rework preparation phase, including public landing entry, self-service company signup onboarding, authentication/login lifecycle, customer/company post-auth entry, company user administration workflow for authorized company operators, and company Pricing & Products configuration workflow for authorized company operators.

#### Scenario: Continuity workflow remains reachable
- **WHEN** a workflow belongs to the continuity-critical scope for rework preparation
- **THEN** users can navigate to at least one corresponding UI flow that starts and completes that workflow

#### Scenario: Non-continuity workflow is absent during cleanup phase
- **WHEN** a workflow is outside the continuity-critical scope
- **THEN** the workflow is not required to be reachable in `WebApp` during the cleanup phase

