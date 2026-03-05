## MODIFIED Requirements

### Requirement: Weekly menu composition
The system SHALL model WeeklyMenu, WeeklyMenuRecipe, and MealSelection entities for menu publication, customer selection, deadline-governed selection finalization, and automatic fallback selection when customer selection is missing.

#### Scenario: Weekly menu recipe ordering
- **WHEN** a WeeklyMenu is defined
- **THEN** the model supports linking recipes via WeeklyMenuRecipe with display order and featured flags

#### Scenario: Selection finalization respects configured deadline
- **WHEN** customer meal choices are submitted for a menu cycle
- **THEN** the selection state reflects whether submission occurred before the configured cutoff and whether fallback auto-selection was required

### Requirement: Customer preference links
The system SHALL model CustomerPreference and CustomerExclusion entities to associate customers with dietary categories and excluded ingredients, and SHALL support applying these preferences/exclusions as constraints in meal candidate generation and filtering.

#### Scenario: Customer dietary preferences
- **WHEN** a customer selects dietary categories
- **THEN** the model stores relationships via CustomerPreference between Customer and DietaryCategory

#### Scenario: Customer ingredient exclusions constrain candidates
- **WHEN** customer meal candidates are generated
- **THEN** recipes containing excluded ingredients are excluded from the candidate set

## ADDED Requirements

### Requirement: Nutrition-aware meal filtering and auto-selection constraints
The system SHALL support per-serving nutritional filter constraints (calories, protein, carbs, fat, fiber, sodium) for customer meal discovery/selection and SHALL enforce configurable non-repetition lookback rules in auto-selection with a default value of 8 weeks.

#### Scenario: Nutritional filters are applied to selection candidates
- **WHEN** nutritional min/max filter criteria are provided
- **THEN** only recipes satisfying all provided nutrient constraints are considered eligible

#### Scenario: Non-repetition lookback is enforced in fallback selection
- **WHEN** auto-selection runs after deadline expiration
- **THEN** recipes served within the configured lookback period are excluded before ranking by rating history

