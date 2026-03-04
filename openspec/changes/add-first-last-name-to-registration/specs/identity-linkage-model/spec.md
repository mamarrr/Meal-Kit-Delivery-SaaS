## MODIFIED Requirements

### Requirement: AppUser identity anchor
The system SHALL model AppUser as the domain identity entity linked to IdentityUser via `IdentityUserId`, and account creation flows MUST populate required identity profile fields (`FirstName`, `LastName`) before persistence.

#### Scenario: Identity linkage
- **WHEN** an AppUser is persisted
- **THEN** it references a required IdentityUserId matching the schema FK

#### Scenario: Required identity names on account creation
- **WHEN** a new account is created through the registration flow
- **THEN** the identity user payload includes non-null `FirstName` and `LastName` values prior to database write
