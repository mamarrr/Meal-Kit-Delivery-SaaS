## ADDED Requirements

### Requirement: Company operators SHALL view deliveries (runs) and orders by date
The system SHALL provide a date-filtered operational list of delivery runs and associated orders within the Delivery Logistics page for the active company tenant.

#### Scenario: Operator filters runs/orders for an operational date
- **WHEN** an authorized operator selects a target delivery date
- **THEN** the system shows delivery runs and associated orders scheduled for that date within the active company

#### Scenario: No runs exist for selected date
- **WHEN** an operator selects a date with no scheduled runs or orders
- **THEN** the system shows an empty-state message and no stale records from other dates

### Requirement: Delivery tracking SHALL support outcomes, notes, proof, and reschedule actions
The system MUST allow authorized company operators to record delivery outcome state (success or failed attempt), capture tracking notes, attach proof metadata, and set reschedule details for failed attempts.

#### Scenario: Operator records successful delivery with proof metadata
- **WHEN** an authorized operator marks a delivery as successful and submits proof metadata
- **THEN** the system persists success status, timestamped tracking details, and proof metadata for that delivery

#### Scenario: Operator records failed attempt with reschedule
- **WHEN** an authorized operator marks a failed attempt and submits reschedule date/time window details
- **THEN** the system persists failed-attempt status and reschedule information
- **AND** subsequent run/order views reflect the updated reschedule state

