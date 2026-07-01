# Intent: Profile Management

## ID
intent-004

## Source Ticket
ECOM-001 (Epic: User Authentication & Account Management) — https://github.com/Mana95/E-commercice-/issues/1

## Status
DRAFT

## Priority
MEDIUM

## Goal
Allow an authenticated customer to view and update their profile, and change their password.

## Affected Modules
- [x] Backend — UserController (profile endpoints), UserService
- [x] Database — extends Users table (FirstName, LastName already implied by validation rules)
- [x] Frontend — Profile page (view/edit), Change Password form

## Acceptance Criteria
- [ ] AC-001: Authenticated user can fetch their profile via GET /api/users/profile
- [ ] AC-002: Authenticated user can update profile (first name, last name) via PUT /api/users/profile
- [ ] AC-003: Update rejects invalid input (e.g. name exceeding max length) with 400
- [ ] AC-004: Authenticated user can change password via PUT /api/users/change-password, requiring current password
- [ ] AC-005: Change-password rejects an incorrect current password with 401/400
- [ ] AC-006: New password must meet the same complexity rules as registration
- [ ] AC-007: Unauthenticated requests to profile endpoints return 401
- [ ] AC-008: Frontend Profile page displays and allows editing name fields
- [ ] AC-009: Frontend Change Password form validates and submits

## Bolts

| Bolt ID | Description | Status |
|---------|-------------|--------|
| 004-bolt-001 | Backend UserService (get/update profile, change password) | PENDING |
| 004-bolt-002 | Backend UserController endpoints + auth guard | PENDING |
| 004-bolt-003 | Backend unit tests (xUnit) | PENDING |
| 004-bolt-004 | Frontend Profile page (view/edit) | PENDING |
| 004-bolt-005 | Frontend Change Password form | PENDING |
| 004-bolt-006 | Frontend unit tests (Vitest) | PENDING |
| 004-bolt-007 | Playwright E2E: login → view profile → update profile → change password | PENDING |

## Discovery Notes
Depends on intent-001 (auth/JWT) being in place first. No existing code found. Mode 2.

## Assumptions
- Profile fields limited to what the epic specifies (FirstName, LastName); email change not in scope per epic validation rules

## Out of Scope
- Avatar/photo upload (not mentioned in epic)
- Account deletion (not mentioned in epic)