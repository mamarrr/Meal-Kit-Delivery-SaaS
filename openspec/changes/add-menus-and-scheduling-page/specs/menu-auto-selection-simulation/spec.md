## ADDED Requirements

### Requirement: Menus & Scheduling SHALL provide auto-selection simulation for a target week
The system SHALL provide a simulation mode that previews how weekly recipe auto-selection would behave for a selected week and configuration set without modifying persisted assignments.

#### Scenario: Operator runs simulation preview
- **WHEN** an authorized operator triggers auto-selection simulation for a target week
- **THEN** the system returns a preview result set of selected recipe candidates by category
- **AND** no weekly assignments are persisted as part of simulation

### Requirement: Simulation SHALL apply configured selection constraints
The simulation engine SHALL apply configured rules including category recipe counts, no-repeat N weeks policy, and deadline-related eligibility filtering where applicable.

#### Scenario: Simulation excludes recently used recipes
- **WHEN** recipes were used inside the configured no-repeat N weeks window
- **THEN** simulation excludes those recipes from candidate selection unless no valid alternatives exist under configured policy

#### Scenario: Simulation respects category quotas
- **WHEN** simulation produces weekly recipe candidates
- **THEN** output satisfies configured recipe counts for each category
- **AND** reports unmet quotas when insufficient eligible recipes exist

### Requirement: Simulation output SHALL include explainability for rejected candidates
The system SHALL provide rule-level diagnostics indicating why recipes were excluded from simulated selection results.

#### Scenario: Operator reviews exclusion reasons
- **WHEN** a simulation run excludes one or more recipes
- **THEN** the output includes per-recipe rejection reasons (for example repeat-window conflict or category overflow)
