# Intent: Email Verification

## ID
intent-002

## Source Ticket
ECOM-001 (Epic: User Authentication & Account Management) — https://github.com/Mana95/E-commercice-/issues/1

## Status
DRAFT

## Priority
HIGH

## Goal
Send a verification email after registration and require the customer's email to be verified before they can log in.

## Affected Modules
- [x] Backend — EmailVerificationService, EmailVerificationToken entity, email sender integration
- [x] Backend — AuthService login gate (block unverified users)
- [x] Database — EmailVerification table
- [x] Frontend — "check your email" screen, verify-email confirmation page

## Acceptance Criteria
- [ ] AC-001: Verification email is sent automatically after successful registration
- [ ] AC-002: Verification link/token is single-use and expires after a set window
- [ ] AC-003: Visiting a valid verification link marks the user as verified
- [ ] AC-004: An expired or already-used verification link returns an appropriate error
- [ ] AC-005: Login attempt by an unverified user is rejected with a clear error (per AC in intent-001 AC-005 family)
- [ ] AC-006: Frontend shows a "verify your email" prompt after registration and a confirmation page after verification

## Bolts

| Bolt ID | Description | Status |
|---------|-------------|--------|
| 002-bolt-001 | Backend EmailVerificationToken model + DB migration | PENDING |
| 002-bolt-002 | Backend EmailVerificationService (generate/send/validate token) | PENDING |
| 002-bolt-003 | Backend endpoint wiring (verify link handling) + login gate | PENDING |
| 002-bolt-004 | Backend unit tests (xUnit) | PENDING |
| 002-bolt-005 | Frontend "check your email" + verify-email confirmation pages | PENDING |
| 002-bolt-006 | Frontend unit tests (Vitest) | PENDING |
| 002-bolt-007 | Playwright E2E: register → receive verification → verify → login succeeds | PENDING |

## Discovery Notes
No existing email-sending infrastructure found. Building from scratch (Mode 2). Email provider not yet chosen — will ask user before introducing a dependency (SMTP/SendGrid/etc.) since it is not in locked decisions.

## Assumptions
- Verification tokens are cryptographically random, single-use, time-limited (e.g. 24h)
- Actual email delivery provider is TBD — pending user decision

## Out of Scope
- Password reset (see intent-003)
- Two-factor authentication