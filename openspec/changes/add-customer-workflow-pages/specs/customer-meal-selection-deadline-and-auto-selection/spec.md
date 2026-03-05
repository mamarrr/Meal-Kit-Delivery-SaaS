## ADDED Requirements

### Requirement: Customer meal selection SHALL enforce configurable decision deadline
The system SHALL enforce a configurable meal selection deadline for customer selections within the relevant scheduling window.

#### Scenario: Customer selects before deadline
- **WHEN** a customer submits meal choices before the configured cutoff
- **THEN** the system accepts and persists the customer-selected meals for that cycle

#### Scenario: Selection attempts after deadline
- **WHEN** a customer attempts to submit choices after the configured cutoff
- **THEN** the system rejects late manual selection and applies auto-selection behavior if not already applied

### Requirement: Auto-selection SHALL prioritize rating history and avoid recent repeats
When a customer misses the deadline, the system SHALL auto-select meals using customer ratings history while avoiding meals served within a configurable lookback period of N weeks (default 8).

#### Scenario: Auto-selection on missed deadline
- **WHEN** the deadline passes without customer selections
- **THEN** the system auto-selects meals from eligible candidates ordered by customer rating preference

#### Scenario: Non-repetition window enforced
- **WHEN** auto-selection evaluates meal candidates
- **THEN** candidates served within the configured N-week lookback are excluded unless no compliant candidate set exists

