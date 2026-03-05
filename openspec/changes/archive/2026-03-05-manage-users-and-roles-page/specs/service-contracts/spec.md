## ADDED Requirements

### Requirement: Company membership management contracts support email-based add and role lifecycle actions
BLL service contracts SHALL expose tenant-scoped operations for CompanyOwner/CompanyAdmin workflows to add existing users by email, update company roles, and remove company memberships.

#### Scenario: Contract supports adding existing user by email
- **WHEN** a caller in company context needs to add a user by email with a selected role
- **THEN** a BLL contract method exists that accepts company context, email, and target role and returns operation outcome

#### Scenario: Contract supports role update for company member
- **WHEN** a caller in company context needs to change a member role
- **THEN** a BLL contract method exists that accepts company context, target membership/user identifier, and target role

#### Scenario: Contract supports membership removal
- **WHEN** a caller in company context needs to remove a member from the company
- **THEN** a BLL contract method exists that accepts company context and target membership/user identifier

### Requirement: Company ownership transfer contract is explicit and owner-scoped
BLL service contracts SHALL define an explicit ownership transfer operation that requires company context and target member identity and is intended for current CompanyOwner authorization paths.

#### Scenario: Ownership transfer contract is available
- **WHEN** ownership needs to be transferred to an existing company member
- **THEN** a dedicated BLL contract method exists for ownership transfer rather than reusing generic role update only

