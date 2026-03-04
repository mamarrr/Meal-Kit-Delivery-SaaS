## Context

Registration currently throws PostgreSQL error `23502` when creating a new identity user because required columns `FirstName` and `LastName` are not populated by the sign-up flow. The database schema already enforces non-null values, so the web registration contract and user creation mapping are out of sync. This change spans UI input collection, server-side validation, and identity persistence behavior.

## Goals / Non-Goals

**Goals:**
- Capture `FirstName` and `LastName` in the registration UI and bind them through the registration view model.
- Enforce server-side validation for both fields before attempting user creation.
- Persist `FirstName` and `LastName` onto the `AppUser` entity during account creation.
- Prevent runtime database constraint failures by failing validation early in the request lifecycle.

**Non-Goals:**
- Reworking tenant onboarding, company creation, or role assignment flows.
- Introducing profile editing features beyond initial registration.
- Changing existing database schema constraints for identity name fields.

## Decisions

1. **Treat first and last name as required registration contract fields**  
   - **Decision:** Add required first-name and last-name inputs to the registration model and form submission contract.  
   - **Rationale:** Database invariants already require non-null values; input contract must align with persistence constraints.  
   - **Alternative considered:** Make DB columns nullable. Rejected because this weakens identity data quality and conflicts with current schema intent.

2. **Validate before persistence, not after database failure**  
   - **Decision:** Use model validation annotations and server-side model state checks to block invalid submissions before calling user manager create operations.  
   - **Rationale:** Produces actionable validation messages, avoids exception-driven control flow, and improves UX reliability.  
   - **Alternative considered:** Catch `PostgresException` and map to user-facing errors. Rejected as brittle, provider-specific, and too late in flow.

3. **Map name fields directly to identity user creation path**  
   - **Decision:** Set `FirstName` and `LastName` when constructing or populating `AppUser` in the registration handler.  
   - **Rationale:** Keeps identity entity and registration contract synchronized at source of truth (account creation).  
   - **Alternative considered:** Post-create enrichment step. Rejected due to risk of partial failures and unnecessary multi-step writes.

4. **Keep scope localized to registration path**  
   - **Decision:** Implement only in registration pages/controllers/view models and related mapping logic.  
   - **Rationale:** Resolves defect with minimal blast radius and preserves current authentication architecture.  
   - **Alternative considered:** Broader profile subsystem update. Rejected as out of scope for bug-fix change.

## Risks / Trade-offs

- **[Risk] Existing tests may fail due to new required fields** → **Mitigation:** Update registration tests/fixtures/builders to include first and last names by default.
- **[Risk] Inconsistent validation between client and server** → **Mitigation:** Keep authoritative validation on server-side DataAnnotations and ensure form helpers render validation metadata.
- **[Risk] Existing external registration clients (if any) omit new fields** → **Mitigation:** Document contract change in release notes/specs and enforce clear validation errors.
- **[Trade-off] Slightly higher registration friction with additional fields** → **Mitigation:** Limit to two essential fields and keep UX labels/help text concise.

## Migration Plan

1. Update registration view model/page model/controller input contract with required `FirstName` and `LastName`.
2. Update registration UI to render first and last name fields with validation messages.
3. Update user creation mapping to persist fields on `AppUser`.
4. Update automated tests and any seed/mock registration payloads.
5. Deploy with no DB migration required (schema already enforces constraints).

**Rollback strategy:** Revert registration UI/model/mapping changes as one unit; no schema rollback required.

## Open Questions

- Are there any API-based or integration-based registration entry points besides the primary web flow that also need these required fields?
- Should we enforce additional normalization (e.g., trimming whitespace, casing rules) now or in a follow-up profile quality change?
