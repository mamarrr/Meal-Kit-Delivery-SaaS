## ADDED Requirements

### Requirement: Customer subscription discovery SHALL list public boxes across companies
The system SHALL provide a customer discovery page that lists public meal box offerings from companies and supports filtering by company, price range, and dietary categories.

#### Scenario: Filter discovery by company, price, and dietary categories
- **WHEN** a customer applies company, min/max price, and dietary category filters
- **THEN** the discovery list only shows public boxes that satisfy all selected filters

### Requirement: Customer SHALL subscribe from discovery and manage current subscriptions
The system SHALL allow a customer to create a subscription from a discovery result and SHALL provide a management page to view active subscriptions and subscription details.

#### Scenario: Subscribe from discovery result
- **WHEN** a customer selects a public box and confirms subscription
- **THEN** a new meal subscription is created for that customer and appears in the manage subscriptions list

#### Scenario: View subscription details from manage subscriptions page
- **WHEN** a customer opens a subscription from the manage subscriptions list
- **THEN** the system displays subscription details including company, selected box, status, and relevant schedule information

### Requirement: Customer SHALL be able to unsubscribe from a managed subscription
The system SHALL allow a customer to unsubscribe from an active meal subscription they own.

#### Scenario: Unsubscribe from active subscription
- **WHEN** a customer confirms unsubscribe on an active managed subscription
- **THEN** the subscription status transitions to inactive/cancelled and is no longer treated as an active subscription

