## ADDED Requirements

### Requirement: Recipe composition relationships
The system SHALL model Recipe, Ingredient, RecipeIngredient, DietaryCategory, and RecipeDietaryCategory entities with their schema-defined relationships.

#### Scenario: Recipe ingredient linkage
- **WHEN** a Recipe includes ingredients
- **THEN** the model represents the relationship through RecipeIngredient linking to both Recipe and Ingredient

### Requirement: Weekly menu composition
The system SHALL model WeeklyMenu, WeeklyMenuRecipe, and MealSelection entities for menu publication and customer selection.

#### Scenario: Weekly menu recipe ordering
- **WHEN** a WeeklyMenu is defined
- **THEN** the model supports linking recipes via WeeklyMenuRecipe with display order and featured flags

### Requirement: Customer preference links
The system SHALL model CustomerPreference and CustomerExclusion entities to associate customers with dietary categories and excluded ingredients.

#### Scenario: Customer dietary preferences
- **WHEN** a customer selects dietary categories
- **THEN** the model stores relationships via CustomerPreference between Customer and DietaryCategory
