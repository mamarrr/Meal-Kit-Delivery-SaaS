## ADDED Requirements

### Requirement: Company users SHALL manage a canonical ingredients catalog
The system SHALL provide tenant-scoped ingredient catalog management so company operators can create, update, and maintain canonical ingredient/tag records used by recipes and exclusion rules.

#### Scenario: Operator creates ingredient catalog entry
- **WHEN** an authorized company operator adds a new ingredient/tag such as `cilantro`
- **THEN** the ingredient is stored as a reusable catalog entry for that company

#### Scenario: Duplicate ingredient names are prevented per tenant
- **WHEN** an authorized operator attempts to create a duplicate canonical ingredient name in the same company
- **THEN** the system rejects the entry
- **AND** returns a validation message indicating uniqueness conflict

### Requirement: Ingredient catalog SHALL support exclusion mapping semantics
The system SHALL allow ingredient catalog entries to be mapped to exclusion semantics used by customer preference/exclusion workflows.

#### Scenario: Operator maps ingredient to exclusion behavior
- **WHEN** an authorized operator maps an ingredient entry to exclusion semantics
- **THEN** the mapping is persisted and available to recipe and customer exclusion logic

#### Scenario: Exclusion mapping is tenant-scoped
- **WHEN** exclusion mappings are queried for an active company
- **THEN** only mappings belonging to that company are returned
- **AND** no cross-tenant mappings are exposed

### Requirement: Exclusion mapping SHALL be applied consistently in recipe-tag validation
The system SHALL validate recipe ingredient/tag assignments against the managed catalog and exclusion mappings to ensure consistent interpretation.

#### Scenario: Recipe editor accepts mapped ingredient tag
- **WHEN** a recipe is edited with an ingredient tag that exists in the company catalog and has valid mapping
- **THEN** the save succeeds with normalized catalog linkage

#### Scenario: Recipe editor rejects unknown ingredient tag for mapped workflows
- **WHEN** a recipe is edited with an ingredient tag that does not exist in the company catalog where mapping is required
- **THEN** the system rejects the save
- **AND** returns a validation message requiring catalog-backed tag selection
