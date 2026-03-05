## ADDED Requirements

### Requirement: Customer dashboard SHALL show personal MealSubscriptions
The system SHALL provide an authenticated customer view where a signed-in user can view only their own MealSubscriptions and related status information.

#### Scenario: Customer opens My MealSubscriptions page
- **WHEN** an authenticated user navigates to the customer subscriptions dashboard
- **THEN** the system displays only MealSubscriptions linked to that user account
- **AND** the system does not display subscriptions belonging to any other user

### Requirement: Customer catalog SHALL allow browsing available MealSubscriptions
The system SHALL provide a customer-facing catalog page that lists subscribable MealSubscriptions available within the currently selected authorized company context.

#### Scenario: Customer browses available subscriptions
- **WHEN** an authenticated customer opens the browse subscriptions page
- **THEN** the system displays available MealSubscription offers for the active authorized company context
- **AND** each offer shows sufficient summary information to decide whether to subscribe

### Requirement: Customer SHALL be able to subscribe to a MealSubscription offer
The system MUST allow an authenticated customer to create a new personal MealSubscription from an available offer and persist it in customer context.

#### Scenario: Customer subscribes successfully
- **WHEN** an authenticated customer selects a valid offer and confirms subscription
- **THEN** the system creates a customer-linked MealSubscription record
- **AND** the new subscription appears in the customer My MealSubscriptions view

