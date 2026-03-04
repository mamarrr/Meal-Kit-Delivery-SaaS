## 1. Registration Contract Updates

- [ ] 1.1 Locate the active registration input model/page model/controller and add required `FirstName` and `LastName` properties with server-side validation attributes.
- [ ] 1.2 Update registration form UI to render first name and last name inputs with validation message display.

## 2. Identity Persistence Alignment

- [ ] 2.1 Update registration user creation mapping so `AppUser`/Identity user receives `FirstName` and `LastName` before `UserManager.CreateAsync`.
- [ ] 2.2 Verify registration success path persists non-null name fields and no longer triggers PostgreSQL `23502` not-null exceptions.

## 3. Regression Safety and Verification

- [ ] 3.1 Update or add registration tests/fixtures to include required name fields and assert validation errors when fields are missing.
- [ ] 3.2 Run solution tests/build checks for the authentication area and confirm no regressions in registration flow.
- [ ] 3.3 Record implementation evidence and prompt trace updates required by project process documentation.
