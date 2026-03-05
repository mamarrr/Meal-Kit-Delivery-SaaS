## ADDED Requirements

### Requirement: Company owner and admin SHALL manage invitations by email
The system SHALL allow users with CompanyOwner or CompanyAdmin role in the active tenant context to invite users to that company by email address.

#### Scenario: Company admin creates invitation
- **WHEN** a CompanyAdmin submits a valid email invitation for the active company
- **THEN** the system creates a tenant-scoped invitation associated with that email and company
- **AND** the invitation is not visible outside that tenant context

### Requirement: Invitation flow SHALL assign company role at onboarding
The system MUST require assigning one tenant role (Admin, Manager, or Employee) when inviting a user and SHALL apply that role when the invited user is linked to the company.

#### Scenario: Owner invites user as Manager
- **WHEN** a CompanyOwner sends an invitation selecting role Manager
- **THEN** the invitation stores the requested role
- **AND** the user receives Manager role in that tenant context when the invitation is accepted or linked

### Requirement: Invitation and role management SHALL enforce authorization boundaries
Only CompanyOwner and CompanyAdmin in the active tenant context SHALL be able to create invitations and assign Admin/Manager/Employee roles.

#### Scenario: Employee attempts to invite a user
- **WHEN** a CompanyEmployee attempts to access invitation creation or role assignment actions
- **THEN** the system denies the action
- **AND** no invitation or role change is persisted

