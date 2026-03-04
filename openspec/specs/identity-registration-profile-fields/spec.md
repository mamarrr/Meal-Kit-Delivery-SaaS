## Purpose
TBD - Defines registration profile field requirements for identity user creation.

## ADDED Requirements

### Requirement: Registration collects required name fields
The system SHALL require `FirstName` and `LastName` input fields during new user registration.

#### Scenario: Registration form is rendered
- **WHEN** an unauthenticated user opens the registration page
- **THEN** the form displays required inputs for first name and last name

#### Scenario: Submission missing first name
- **WHEN** a registration request is submitted without `FirstName`
- **THEN** the system rejects the request with a validation error and does not create a user

#### Scenario: Submission missing last name
- **WHEN** a registration request is submitted without `LastName`
- **THEN** the system rejects the request with a validation error and does not create a user

### Requirement: Registration persists name fields to identity user
The system SHALL persist provided `FirstName` and `LastName` values to the created identity user record.

#### Scenario: Successful registration with valid names
- **WHEN** a user submits registration with valid first name, last name, email, and password
- **THEN** the created user record stores non-null `FirstName` and `LastName` matching submitted values

#### Scenario: Database constraint compliance
- **WHEN** registration succeeds
- **THEN** persistence does not violate non-null constraints for `AspNetUsers.FirstName` and `AspNetUsers.LastName`
