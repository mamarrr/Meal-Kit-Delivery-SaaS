## ADDED Requirements

### Requirement: Company operators SHALL manage delivery zones and schedules with capacity constraints
The system SHALL provide tenant-scoped zone and schedule management capabilities that include delivery day assignment, delivery time windows, and per-window capacity limits for each delivery zone.

#### Scenario: Operator views zone list and schedule details
- **WHEN** an authorized operator opens the Delivery Logistics page
- **THEN** the system displays all active company delivery zones
- **AND** each zone includes visible schedule attributes for delivery days, time windows, and capacity limits

#### Scenario: Operator updates capacity limits for a zone window
- **WHEN** an authorized operator submits an updated capacity limit for a specific zone time window
- **THEN** the system persists the new capacity value for that company zone window
- **AND** subsequent page loads show the updated capacity

### Requirement: Schedule and capacity updates SHALL validate operational consistency
The system MUST validate zone schedule updates to prevent invalid or contradictory delivery window configurations within the same zone.

#### Scenario: Invalid schedule window is rejected
- **WHEN** an operator submits a schedule window with invalid time boundaries
- **THEN** the system rejects the update with a validation error

#### Scenario: Capacity value outside allowed bounds is rejected
- **WHEN** an operator submits a capacity limit that is below minimum or above configured maximum policy
- **THEN** the system rejects the update and preserves the existing persisted value

