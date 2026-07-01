# Intent: User Authentication — Register, Login, Logout

## ID
intent-001

## Source Ticket
ECOM-001 (Epic: User Authentication & Account Management) — https://github.com/Mana95/E-commercice-/issues/1

## Status
DRAFT

## Priority
HIGH

## Goal
Build core authentication: registration, login, logout, and JWT access + refresh token issuance. Users must be able to create an account with email/password and log in to receive a JWT access token (plus refresh token) for accessing protected resources.

## Affected Modules
- [x] Frontend — Login page, Register page, auth service, auth context
- [x] Backend — AuthController, AuthService, User model, JWT middleware
- [x] Database — Users table (Id, Email, PasswordHash, CreatedAt)
- [x] Auth — JWT token generation and validation

## Acceptance Criteria

- [ ] AC-001: User can register with email and password; API returns 201 with user ID
- [ ] AC-002: Registration with duplicate email returns 409 Conflict
- [ ] AC-003: Registration with invalid email or weak password returns 400 with validation errors
- [ ] AC-004: User can login with valid credentials; API returns 200 with JWT access token
- [ ] AC-005: Login with wrong credentials returns 401 Unauthorized
- [ ] AC-006: JWT token contains user ID and email claims, expires in 1 hour
- [x] AC-007: Protected endpoints reject requests without valid JWT (401) — validated via ProductController (intent-005, 005-bolt-003)
- [ ] AC-008: Frontend login form submits credentials and stores token on success
- [ ] AC-009: Frontend register form submits data and redirects to login on success
- [ ] AC-010: Frontend shows validation errors from API
- [ ] AC-011: User can log out; refresh token is invalidated
- [ ] AC-012: User can obtain new access token via POST /api/auth/refresh-token using a valid refresh token

## Bolts

| Bolt ID | Description | Status |
|---------|-------------|--------|
| 001-bolt-001 | Backend User + RefreshToken models + DB migration + DbContext | PENDING |
| 001-bolt-002 | Backend AuthService (register + login + JWT access/refresh generation) | PENDING |
| 001-bolt-003 | Backend AuthController (register/login/logout/refresh-token) + JWT middleware config | PENDING |
| 001-bolt-004 | Backend unit tests (xUnit) | PENDING |
| 001-bolt-005 | Frontend auth service + types | PENDING |
| 001-bolt-006 | Frontend Register page + Login page | PENDING |
| 001-bolt-007 | Frontend auth context + protected route + logout | PENDING |
| 001-bolt-008 | Frontend unit tests (Vitest) | PENDING |
| 001-bolt-009 | Playwright E2E: register → verify gate → login → logout flow | PENDING |

## Discovery Notes
No existing auth code found. Building from scratch (Mode 2).

## Assumptions
- Password hashing via BCrypt
- JWT secret stored in appsettings (not hardcoded)
- Refresh tokens persisted in DB (RefreshToken entity), rotated on use
- Password complexity per epic: min 8 chars, upper+lower+number+special char
- Email verification required before first login (see intent-002) — login blocked until verified

## Out of Scope
- Password reset / forgot password (see intent-003)
- Email verification (see intent-002)
- Profile management (see intent-004)
- Social/OAuth login
- Two-factor authentication
- Role-based authorization (future intent)
