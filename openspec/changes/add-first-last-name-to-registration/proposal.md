## Why

User registration currently fails with a PostgreSQL not-null constraint violation because `AspNetUsers.FirstName` is required but not collected during sign-up. This blocks onboarding and prevents creating new user accounts, so registration must capture required profile fields now.

## What Changes

- Update registration requirements to collect both first name and last name for every new user.
- Persist provided first name and last name into the identity user record at registration time.
- Enforce validation so registration cannot submit without valid first and last names.
- Align registration UX and backend contract with database constraints to prevent `23502` null violations.

## Capabilities

### New Capabilities
- `identity-registration-profile-fields`: Registration flow captures, validates, and saves required user profile fields (`FirstName`, `LastName`) for new accounts.

### Modified Capabilities
- `identity-linkage-model`: Require non-null identity name fields during account creation so Identity persistence remains consistent with schema constraints.

## Impact

- Affected systems: Web registration UI/view model, Identity registration handler/controller/page model, and user creation mapping logic.
- Affected data contract: Required inputs for new account creation now include first and last name.
- Affected behavior: Registration failures caused by missing `FirstName`/`LastName` are eliminated by pre-persistence validation and field capture.
