## 1. Navigation Eligibility and Placement

- [x] 1.1 Implement a shared eligibility check for "authenticated and not in any system-level role" for sidebar CTA visibility.
- [x] 1.2 Update the shared left sidebar layout/partial to render a bottom-positioned "Register new company" navigation action when eligibility is true.
- [x] 1.3 Ensure users with any system-level role never see the "Register new company" sidebar action.

## 2. Onboarding Entry Routing Behavior

- [x] 2.1 Wire the sidebar "Register new company" action to the existing self-service tenant onboarding entry route.
- [x] 2.2 Verify onboarding entry works for eligible users who already belong to one or more companies.
- [x] 2.3 Confirm onboarding initiation preserves existing memberships and starts a new company registration attempt.

## 3. Verification and Regression Coverage

- [x] 3.1 Add/adjust tests for sidebar visibility matrix (non-system users vs system-role users, including mixed-role identities).
- [x] 3.2 Add/adjust tests confirming the CTA appears in the sidebar bottom section and navigates to onboarding.
- [x] 3.3 Run targeted test suite and fix regressions related to shell navigation and onboarding entry behavior.
