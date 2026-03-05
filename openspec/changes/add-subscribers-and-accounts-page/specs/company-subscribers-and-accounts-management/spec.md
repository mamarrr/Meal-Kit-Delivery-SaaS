## ADDED Requirements

### Requirement: Company workspace SHALL provide Subscribers & Accounts list operations
The system SHALL provide a company-scoped Subscribers & Accounts page that lists subscribers for the active company and supports search plus filters for subscription status, subscriber tier, and delivery zone.

#### Scenario: Authorized company user opens subscriber list
- **WHEN** a CompanyOwner, CompanyAdmin, CompanyManager, or CompanyEmployee opens the Subscribers & Accounts page in company view
- **THEN** the system shows only subscribers belonging to the active company
- **AND** the list includes controls for search, status filter, tier filter, and delivery zone filter

#### Scenario: User filters by status tier and zone
- **WHEN** a company user applies status, tier, and delivery zone filters
- **THEN** the list shows only subscribers matching all selected filters within the active company

#### Scenario: User searches subscribers
- **WHEN** a company user enters a search term for subscriber identity fields on the list
- **THEN** the list returns matching subscribers scoped to the active company

### Requirement: Subscriber details SHALL present operational account context in one page flow
The system SHALL present a subscriber detail view from the list that includes addresses, assigned delivery zone, plan/box details, preferences, exclusions, and historical operational context without requiring navigation to unrelated modules for core account triage.

#### Scenario: User opens subscriber details from list
- **WHEN** a company user selects a subscriber row from the Subscribers & Accounts list
- **THEN** the system shows subscriber details including addresses, delivery zone, and plan/box information

#### Scenario: User reviews preference and exclusion profile
- **WHEN** a company user views subscriber details
- **THEN** the system shows dietary preferences and ingredient exclusions configured for that subscriber

### Requirement: Preferences and exclusions SHALL support explicit ingredient restrictions
The system SHALL display subscriber preference/exclusion data including explicit exclusions such as shellfish, pork, and cilantro when present.

#### Scenario: Subscriber has configured exclusions
- **WHEN** a subscriber detail has configured ingredient exclusions
- **THEN** the detail view shows each configured exclusion including shellfish, pork, cilantro, and other saved exclusions

#### Scenario: Subscriber has no exclusions
- **WHEN** a subscriber detail has no configured exclusions
- **THEN** the detail view shows an explicit empty-state indicator for exclusions

### Requirement: Ratings history SHALL be visible for auto-selection auditability
The system SHALL provide ratings history in subscriber details so company users can inspect the historical basis used by meal auto-selection logic.

#### Scenario: Ratings history exists
- **WHEN** a company user opens ratings history for a subscriber
- **THEN** the system shows historical ratings entries in descending chronological order

#### Scenario: Ratings history missing
- **WHEN** a subscriber has no ratings history
- **THEN** the system shows an explicit empty-state indicator for ratings history

### Requirement: Subscription lifecycle events SHALL be visible in subscriber account details
The system SHALL display subscriber lifecycle history that includes pauses, skips, and cancellations for the active company subscriber account.

#### Scenario: Subscriber has pause skip cancellation records
- **WHEN** a company user views lifecycle history in subscriber details
- **THEN** the system shows pause, skip, and cancellation events with their recorded effective dates and statuses

#### Scenario: Subscriber has no lifecycle events
- **WHEN** no pauses skips or cancellations are recorded for a subscriber
- **THEN** the lifecycle section shows an explicit empty-state indicator
