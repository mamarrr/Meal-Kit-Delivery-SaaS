## ADDED Requirements

### Requirement: Delivery lifecycle entities
The system SHALL model Delivery, DeliveryAttempt, DeliveryStatus, and DeliveryAttemptResult entities according to the schema.

#### Scenario: Delivery attempts tracking
- **WHEN** a delivery attempt is recorded
- **THEN** the model links DeliveryAttempt to Delivery and DeliveryAttemptResult

### Requirement: Delivery logistics entities
The system SHALL model DeliveryZone and DeliveryWindow entities for zone/day/time capacity definitions.

#### Scenario: Delivery window association
- **WHEN** defining delivery windows for a zone
- **THEN** the model links DeliveryWindow to its DeliveryZone

### Requirement: Quality complaint entities
The system SHALL model QualityComplaint, QualityComplaintType, and QualityComplaintStatus entities for complaint tracking.

#### Scenario: Complaint linkage
- **WHEN** a quality complaint is recorded
- **THEN** the model links the complaint to Company, Customer, Delivery, and status/type lookups
