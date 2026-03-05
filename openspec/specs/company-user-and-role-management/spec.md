## ADDED Requirements

### Requirement: Authorized company administrators can add existing users to a company by email
The system SHALL provide a company-scoped user management action that allows a signed-in CompanyOwner or CompanyAdmin to add a user to the active company by entering an email address and selecting one of the company roles CompanyAdmin, CompanyManager, or CompanyEmployee. The system MUST only create membership when an existing platform account for that email is found.

#### Scenario: Add existing account with selected role
- **WHEN** a CompanyOwner or CompanyAdmin submits an email that matches an existing platform account and selects a valid company role
- **THEN** the system creates a company membership for that account in the active company with the selected role immediately

#### Scenario: Reject add when email does not exist
- **WHEN** a CompanyOwner or CompanyAdmin submits an email that does not match any existing platform account
- **THEN** the system does not create membership and returns a validation error indicating the email account must already exist

#### Scenario: Prevent duplicate membership add
- **WHEN** a CompanyOwner or CompanyAdmin attempts to add an account that is already a member of the active company
- **THEN** the system does not create a duplicate membership and returns an error indicating the user is already in the company

### Requirement: Authorized company administrators can manage member roles
The system SHALL allow CompanyOwner and CompanyAdmin to change roles of existing company members between CompanyAdmin, CompanyManager, and CompanyEmployee subject to authorization and ownership guardrails.

#### Scenario: Owner changes member role
- **WHEN** a CompanyOwner changes a company member role from CompanyEmployee to CompanyManager
- **THEN** the system persists the updated role for that member in the active company

#### Scenario: Admin changes non-owner member role
- **WHEN** a CompanyAdmin changes the role of a member who is not CompanyOwner
- **THEN** the system persists the new allowed role assignment

#### Scenario: Prevent non-owner from changing owner role via standard role management
- **WHEN** a CompanyAdmin attempts to change the role of the current CompanyOwner through role management actions
- **THEN** the system denies the operation and requires ownership transfer flow for owner changes

### Requirement: Authorized company administrators can remove users from company membership
The system SHALL allow CompanyOwner and CompanyAdmin to remove existing members from the active company while preserving role and ownership constraints.

#### Scenario: Remove non-owner member
- **WHEN** a CompanyOwner or CompanyAdmin removes a member who is not the current CompanyOwner
- **THEN** the system revokes that user’s membership in the active company

#### Scenario: Prevent removing current owner through standard remove action
- **WHEN** a CompanyOwner or CompanyAdmin attempts to remove the current CompanyOwner via membership removal
- **THEN** the system denies the action and indicates ownership transfer is required first

### Requirement: Tenant boundaries and authorization are enforced for all user-management actions
The system MUST enforce company scoping and role-based authorization for add, role update, and removal operations so that users cannot operate outside their active company or permission level.

#### Scenario: Block unauthorized company manager from user administration
- **WHEN** a signed-in CompanyManager attempts to access user-management actions
- **THEN** the system denies access

#### Scenario: Block cross-tenant target operations
- **WHEN** an authorized actor attempts to mutate membership using an identifier not belonging to the active company context
- **THEN** the system denies the operation and no data is changed
