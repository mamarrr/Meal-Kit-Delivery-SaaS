## ADDED Requirements

### Requirement: AppUser identity anchor
The system SHALL model AppUser as the domain identity entity linked to IdentityUser via `IdentityUserId`.

#### Scenario: Identity linkage
- **WHEN** an AppUser is persisted
- **THEN** it references a required IdentityUserId matching the schema FK

### Requirement: Company membership mapping
The system SHALL model CompanyAppUser to link AppUser to Company with a CompanyRole and ownership flag.

#### Scenario: Company membership assignment
- **WHEN** a user is added to a company
- **THEN** a CompanyAppUser entity links AppUser, Company, and CompanyRole with required fields

### Requirement: Customer membership mapping
The system SHALL model CustomerAppUser to link AppUser to Customer accounts.

#### Scenario: Customer account linkage
- **WHEN** an AppUser acts as a customer
- **THEN** a CustomerAppUser entity links the AppUser to the Customer record
