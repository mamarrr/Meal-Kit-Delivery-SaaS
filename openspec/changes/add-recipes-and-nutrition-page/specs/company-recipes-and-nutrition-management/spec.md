## ADDED Requirements

### Requirement: Company users SHALL access a dedicated Recipes & Nutrition workspace
The system SHALL provide a tenant-scoped Recipes & Nutrition page or subpage set in company view where authorized company operators can manage recipe metadata and nutrition details.

#### Scenario: Authorized operator opens Recipes & Nutrition
- **WHEN** a signed-in CompanyOwner, CompanyAdmin, or CompanyManager navigates company operations
- **THEN** the UI shows an entry to Recipes & Nutrition
- **AND** the Recipes & Nutrition route loads for the active company context

#### Scenario: Unauthorized operator is denied Recipes & Nutrition access
- **WHEN** a user without required company operational permissions attempts to open Recipes & Nutrition routes
- **THEN** the UI and backend authorization deny access

### Requirement: Recipe editor SHALL support ingredients, allergens/tags, and dietary categories
The system SHALL allow authorized company users to create and edit recipes with structured ingredient associations, allergen/tag metadata, and dietary category assignments.

#### Scenario: Operator saves recipe ingredient and tag metadata
- **WHEN** an authorized operator submits recipe editor changes including ingredients and allergen/tag values
- **THEN** the system persists those values for the active company recipe

#### Scenario: Operator assigns dietary categories
- **WHEN** an authorized operator assigns one or more dietary categories to a recipe
- **THEN** the system persists recipe-category associations for that company

### Requirement: Recipe editor SHALL capture nutrition per serving
The system SHALL store and expose per-serving nutrition values for each recipe, including calories, protein, carbs, fat, fiber, and sodium.

#### Scenario: Operator saves complete nutrition values
- **WHEN** an authorized operator provides calories, protein, carbs, fat, fiber, and sodium for a recipe
- **THEN** the recipe stores all nutrition-per-serving fields
- **AND** subsequent recipe reads return those values

#### Scenario: Missing required nutrition values are rejected
- **WHEN** an authorized operator attempts to save a recipe without required nutrition-per-serving fields
- **THEN** the system rejects the save
- **AND** returns field-level validation errors identifying missing values
