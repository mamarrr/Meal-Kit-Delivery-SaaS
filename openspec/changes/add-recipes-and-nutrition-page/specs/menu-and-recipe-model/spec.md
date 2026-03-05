## MODIFIED Requirements

### Requirement: Recipe composition relationships
The system SHALL model Recipe, Ingredient, RecipeIngredient, DietaryCategory, and RecipeDietaryCategory entities with their schema-defined relationships, and SHALL include structured recipe metadata for allergen/tags and per-serving nutrition values (calories, protein, carbs, fat, fiber, sodium).

#### Scenario: Recipe ingredient linkage
- **WHEN** a Recipe includes ingredients
- **THEN** the model represents the relationship through RecipeIngredient linking to both Recipe and Ingredient

#### Scenario: Recipe nutrition and tag metadata is represented
- **WHEN** a recipe is stored in the domain model
- **THEN** per-serving nutrition values and allergen/tag metadata are available as structured fields associated with that recipe

### Requirement: Customer preference links
The system SHALL model CustomerPreference and CustomerExclusion entities to associate customers with dietary categories and excluded ingredients, and SHALL support excluded ingredient references that can be resolved against company-managed ingredient catalog entries.

#### Scenario: Customer dietary preferences
- **WHEN** a customer selects dietary categories
- **THEN** the model stores relationships via CustomerPreference between Customer and DietaryCategory

#### Scenario: Customer ingredient exclusions resolve to catalog tags
- **WHEN** a customer exclusion is stored for a company-managed ingredient tag
- **THEN** the model can link or resolve that exclusion against the company ingredient catalog semantics
