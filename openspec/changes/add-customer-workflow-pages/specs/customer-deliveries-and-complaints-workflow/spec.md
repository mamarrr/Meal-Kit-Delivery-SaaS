## ADDED Requirements

### Requirement: Customer deliveries page SHALL expose delivery history and outcomes
The system SHALL provide a customer deliveries page that lists deliveries associated with the customer's subscriptions and shows delivery outcomes including successful and failed attempts.

#### Scenario: Customer views delivery history
- **WHEN** a customer opens the deliveries page
- **THEN** the system displays delivery entries for that customer with delivery date/time window and outcome state

### Requirement: Customer complaints page SHALL support complaint creation and status visibility
The system SHALL provide a customer complaints page where a customer can submit quality complaints related to delivered meals and view complaint status for their own complaints.

#### Scenario: Submit complaint for delivered meal
- **WHEN** a customer submits a complaint tied to a delivered meal
- **THEN** the complaint is created and appears in the customer complaint list with an initial status

#### Scenario: View complaint status updates
- **WHEN** a customer opens complaints history
- **THEN** the system displays current status and timeline metadata for the customer's complaints

