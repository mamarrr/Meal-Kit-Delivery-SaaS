## MODIFIED Requirements

### Requirement: UI SHALL cover all required business workflows from configuration context
The WebApp SHALL expose continuity-critical workflows during the UI rework preparation phase, including public landing entry, self-service company signup onboarding, authentication/login lifecycle, customer/company post-auth entry, company user administration workflow for authorized company operators, and company Recipes & Nutrition workflows for authorized operational roles.

#### Scenario: Continuity workflow remains reachable
- **WHEN** a workflow belongs to the continuity-critical scope for rework preparation
- **THEN** users can navigate to at least one corresponding UI flow that starts and completes that workflow

#### Scenario: Non-continuity workflow is absent during cleanup phase
- **WHEN** a workflow is outside the continuity-critical scope
- **THEN** the workflow is not required to be reachable in `WebApp` during the cleanup phase

### Requirement: Company operations navigation SHALL include Recipes & Nutrition entry for authorized roles
The WebApp SHALL include a company-scoped Recipes & Nutrition navigation entry in company operations view that is visible to CompanyOwner, CompanyAdmin, and CompanyManager and hidden or inaccessible for unauthorized roles.

#### Scenario: Authorized role sees Recipes & Nutrition entry
- **WHEN** a signed-in CompanyOwner, CompanyAdmin, or CompanyManager accesses company operations navigation
- **THEN** the UI shows a link to the Recipes & Nutrition workflow

#### Scenario: Unauthorized role cannot access Recipes & Nutrition route
- **WHEN** an unauthorized user attempts to access Recipes & Nutrition route directly
- **THEN** the UI and backend authorization deny access
