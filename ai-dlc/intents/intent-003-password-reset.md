# Intent: Forgot Password / Reset Password

## ID
intent-003

## Source Ticket
ECOM-001 (Epic: User Authentication & Account Management) — https://github.com/Mana95/E-commercice-/issues/1

## Status
DRAFT

## Priority
HIGH

## Goal
Allow a customer who forgot their password to request a reset link by email and set a new password.

## Affected Modules
- [x] Backend — PasswordResetService, PasswordResetToken entity, email sender integration
- [x] Database — PasswordResetToken table
- [x] Frontend — Forgot Password page, Reset Password page

## Acceptance Criteria
- [ ] AC-001: Customer can request a password reset via POST /api/auth/forgot-password using their email
- [ ] AC-002: Reset email is sent only if the email exists (response does not leak whether the account exists)
- [ ] AC-003: Reset token is single-use and expires after a set window
- [ ] AC-004: Customer can set a new password via POST /api/auth/reset-password using a valid token
- [ ] AC-005: New password must meet the same complexity rules as registration
- [ ] AC-006: Expired or already-used reset tokens return an appropriate error
- [ ] AC-007: Frontend Forgot Password page collects email and shows confirmation
- [ ] AC-008: Frontend Reset Password page collects new password and shows validation errors

## Bolts

| Bolt ID | Description | Status |
|---------|-------------|--------|
| 003-bolt-001 | Backend PasswordResetToken model + DB migration | PENDING |
| 003-bolt-002 | Backend PasswordResetService (generate/send/validate/consume token) | PENDING |
| 003-bolt-003 | Backend AuthController endpoints (forgot-password, reset-password) | PENDING |
| 003-bolt-004 | Backend unit tests (xUnit) | PENDING |
| 003-bolt-005 | Frontend Forgot Password page + Reset Password page | PENDING |
| 003-bolt-006 | Frontend unit tests (Vitest) | PENDING |
| 003-bolt-007 | Playwright E2E: forgot password → reset link → new password → login | PENDING |

## Discovery Notes
Depends on the same email delivery mechanism as intent-002 (email verification). No existing code found. Mode 2.

## Assumptions
- Reset tokens are cryptographically random, single-use, time-limited (e.g. 1h)
- Email provider decision shared with intent-002

## Out of Scope
- Email verification (see intent-002)
- Profile/change-password while logged in (see intent-004)