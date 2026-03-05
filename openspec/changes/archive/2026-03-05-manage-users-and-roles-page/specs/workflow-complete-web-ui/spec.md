## ADDED Requirements

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

