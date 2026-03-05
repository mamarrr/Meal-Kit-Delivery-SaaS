## ADDED Requirements

### Requirement: Customer preferences and exclusions SHALL use subscribed-company catalogs
The system SHALL allow customers to manage dietary preferences and ingredient exclusions using dietary categories and ingredients derived from companies the customer is currently subscribed to.

#### Scenario: Preference options are scoped to subscribed companies
- **WHEN** a customer opens the preferences/exclusions page
- **THEN** the selectable dietary categories and ingredients come only from the catalogs of currently subscribed companies

### Requirement: Customer SHALL persist dietary preferences and ingredient exclusions
The system SHALL persist customer-selected dietary preferences and ingredient exclusions and apply them to meal candidate filtering.

#### Scenario: Save preferences and exclusions
- **WHEN** a customer submits updated dietary preferences and ingredient exclusions
- **THEN** the saved values are persisted and visible on subsequent page loads

### Requirement: Nutrition criteria SHALL be available for customer meal filtering
The system SHALL support customer nutritional filter criteria per serving for calories, protein, carbs, fat, fiber, and sodium using range-based constraints.

#### Scenario: Filter meal candidates by macro criteria
- **WHEN** a customer sets nutritional min/max ranges and executes meal filtering
- **THEN** returned meal candidates satisfy the provided nutrient constraints per serving

