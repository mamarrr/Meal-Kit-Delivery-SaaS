## ADDED Requirements

### Requirement: Tenant SHALL configure delivery fee entries
The system SHALL allow authorized tenant operators to create, update, activate, and deactivate delivery fee entries used in Pricing & Products configuration.

#### Scenario: Operator creates delivery fee entry
- **WHEN** an authorized operator submits a valid delivery fee configuration
- **THEN** the system stores a tenant-scoped delivery fee record and shows it in the fees list

#### Scenario: Operator deactivates a delivery fee entry
- **WHEN** an authorized operator deactivates an existing delivery fee
- **THEN** the fee remains stored for history but is excluded from active pricing configuration

### Requirement: Tenant SHALL configure discount entries
The system SHALL allow authorized tenant operators to manage discount entries, including discount type and amount/value, within tenant pricing configuration.

#### Scenario: Operator adds a percentage discount
- **WHEN** an authorized operator creates a percentage discount with a valid percentage
- **THEN** the system persists the tenant-scoped discount entry and displays it in discount configuration

#### Scenario: Operator adds a fixed-amount discount
- **WHEN** an authorized operator creates a fixed-amount discount with a valid currency value
- **THEN** the system persists the discount entry with fixed-amount type metadata

### Requirement: Fee and discount configurations SHALL be validated and non-destructive
The system SHALL validate fee and discount values before save and SHALL use non-destructive state changes for removed pricing adjustments.

#### Scenario: Invalid discount value is rejected
- **WHEN** an operator submits a discount value outside allowed bounds for the selected type
- **THEN** the system rejects the save and returns validation errors

#### Scenario: Removed adjustment is soft-deactivated
- **WHEN** an operator removes a fee or discount from active configuration
- **THEN** the record is marked inactive and remains available for audit/history contexts

