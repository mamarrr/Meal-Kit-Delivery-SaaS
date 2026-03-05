## ADDED Requirements

### Requirement: Complaint-based escalation rules SHALL be configurable per company
The system SHALL allow authorized company operators to configure complaint-based escalation policies that influence delivery handling priority, including freshest-stock prioritization and earliest-slot prioritization.

#### Scenario: Operator configures escalation policy options
- **WHEN** a CompanyAdmin or CompanyManager updates escalation rule settings
- **THEN** the system persists the selected policy options for the active company
- **AND** the Delivery Logistics page reflects the saved rule configuration

#### Scenario: Unauthorized role attempts escalation rule changes
- **WHEN** a role without escalation-rule edit permission attempts to modify complaint-based policies
- **THEN** the system denies the update and preserves existing settings

### Requirement: Escalation policy SHALL be applied deterministically for complaint-driven prioritization
The system MUST evaluate complaint-linked delivery prioritization using deterministic policy order so that repeated complaint handling is auditable and consistent.

#### Scenario: Freshest stock policy is active for complaint escalation
- **WHEN** a complaint-triggered escalation is evaluated and freshest-stock policy is enabled
- **THEN** the system prioritizes eligible fulfillment options by freshest stock ordering before secondary tie-breakers

#### Scenario: Earliest slot policy is active for complaint escalation
- **WHEN** a complaint-triggered escalation is evaluated and earliest-slot policy is enabled
- **THEN** the system prioritizes available delivery options by earliest deliverable slot within configured constraints

