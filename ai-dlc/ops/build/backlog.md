# Build Backlog

## How to Read This
- PENDING   → not started
- IN PROGRESS → agent currently working on this
- BLOCKED   → waiting on something (see notes)
- DONE      → built + validated + all criteria passed
- BUG       → validation failed, bug lifecycle triggered

---

## Active Bolts

| ID | Bolt | Intent | Status | Complexity | Notes |
|----|------|--------|--------|------------|-------|
| 001-bolt-001 | Backend User + RefreshToken models + DB migration + DbContext | intent-001 (ECOM-001) | DONE | M | src/backend scaffolded (.NET 8), tests/backend scaffolded (xUnit), migration InitialAuth generated |
| 001-bolt-002 | Backend AuthService (register + login + JWT access/refresh generation) | intent-001 (ECOM-001) | DONE | M | Email-verification login gate deferred to intent-002 (#3) |
| 001-bolt-003 | Backend AuthController (register/login/logout/refresh-token) + JWT middleware | intent-001 (ECOM-001) | DONE | M | AC-007 (401 on missing JWT) validated once a protected endpoint exists in intent-004 |
| 001-bolt-004 | Backend unit tests (xUnit) | intent-001 (ECOM-001) | DONE | S | Written alongside 001-bolt-002/003 (22/22 passing) |
| 001-bolt-005 | Frontend auth service + types | intent-001 (ECOM-001) | DONE | S | src/frontend scaffolded (React 18 + TS + Tailwind + Vitest), 4/4 tests pass |
| 001-bolt-006 | Frontend Register page + Login page | intent-001 (ECOM-001) | DONE | M | react-router-dom added (ADR-006, user-confirmed); 8/8 tests pass |
| 001-bolt-007 | Frontend auth context + protected route + logout | intent-001 (ECOM-001) | DONE | M | 13/13 tests pass, build clean |
| 001-bolt-008 | Frontend unit tests (Vitest) | intent-001 (ECOM-001) | DONE | S | Written alongside 001-bolt-005/006/007 (13/13 passing) |
| 001-bolt-009 | Playwright E2E: register → login → logout | intent-001 (ECOM-001) | PENDING | S | |
| 002-bolt-001 | Backend EmailVerificationToken model + DB migration | intent-002 (ECOM-001) | PENDING | S | Blocked on email provider decision |
| 002-bolt-002 | Backend EmailVerificationService | intent-002 (ECOM-001) | PENDING | M | Blocked on email provider decision |
| 002-bolt-003 | Backend verify endpoint + login gate | intent-002 (ECOM-001) | PENDING | S | |
| 002-bolt-004 | Backend unit tests (xUnit) | intent-002 (ECOM-001) | PENDING | S | |
| 002-bolt-005 | Frontend check-email + verify-email pages | intent-002 (ECOM-001) | PENDING | S | |
| 002-bolt-006 | Frontend unit tests (Vitest) | intent-002 (ECOM-001) | PENDING | S | |
| 002-bolt-007 | Playwright E2E: register → verify → login | intent-002 (ECOM-001) | PENDING | S | |
| 003-bolt-001 | Backend PasswordResetToken model + DB migration | intent-003 (ECOM-001) | PENDING | S | Blocked on email provider decision |
| 003-bolt-002 | Backend PasswordResetService | intent-003 (ECOM-001) | PENDING | M | Blocked on email provider decision |
| 003-bolt-003 | Backend forgot/reset-password endpoints | intent-003 (ECOM-001) | PENDING | S | |
| 003-bolt-004 | Backend unit tests (xUnit) | intent-003 (ECOM-001) | PENDING | S | |
| 003-bolt-005 | Frontend Forgot Password + Reset Password pages | intent-003 (ECOM-001) | PENDING | S | |
| 003-bolt-006 | Frontend unit tests (Vitest) | intent-003 (ECOM-001) | PENDING | S | |
| 003-bolt-007 | Playwright E2E: forgot → reset → login | intent-003 (ECOM-001) | PENDING | S | |
| 004-bolt-001 | Backend UserService (profile get/update, change password) | intent-004 (ECOM-001) | PENDING | S | |
| 004-bolt-002 | Backend UserController endpoints + auth guard | intent-004 (ECOM-001) | PENDING | S | |
| 004-bolt-003 | Backend unit tests (xUnit) | intent-004 (ECOM-001) | PENDING | S | |
| 004-bolt-004 | Frontend Profile page | intent-004 (ECOM-001) | PENDING | S | |
| 004-bolt-005 | Frontend Change Password form | intent-004 (ECOM-001) | PENDING | S | |
| 004-bolt-006 | Frontend unit tests (Vitest) | intent-004 (ECOM-001) | PENDING | S | |
| 004-bolt-007 | Playwright E2E: profile view/update/change-password | intent-004 (ECOM-001) | PENDING | S | |
| 005-bolt-001 | Backend Product + Category + Brand models + DB migration | intent-005 (ECOM-002) | PENDING | M | |
| 005-bolt-002 | Backend ProductService (create/update/archive/get/list) | intent-005 (ECOM-002) | PENDING | M | |
| 005-bolt-003 | Backend ProductController + auth guard | intent-005 (ECOM-002) | PENDING | M | |
| 005-bolt-004 | Backend unit tests (xUnit) | intent-005 (ECOM-002) | PENDING | S | |
| 005-bolt-005 | Frontend admin product list page | intent-005 (ECOM-002) | PENDING | S | |
| 005-bolt-006 | Frontend admin create/edit product form | intent-005 (ECOM-002) | PENDING | M | |
| 005-bolt-007 | Frontend unit tests (Vitest) | intent-005 (ECOM-002) | PENDING | S | |
| 005-bolt-008 | Playwright E2E: create → edit → archive product | intent-005 (ECOM-002) | PENDING | S | |
| 006-bolt-001 | Backend ProductImage model + DB migration | intent-006 (ECOM-002) | PENDING | S | Blocked on file storage provider decision |
| 006-bolt-002 | Backend ProductImageService | intent-006 (ECOM-002) | PENDING | M | Blocked on file storage provider decision |
| 006-bolt-003 | Backend image endpoints on ProductController | intent-006 (ECOM-002) | PENDING | S | |
| 006-bolt-004 | Backend unit tests (xUnit) | intent-006 (ECOM-002) | PENDING | S | |
| 006-bolt-005 | Frontend image upload + gallery UI | intent-006 (ECOM-002) | PENDING | S | |
| 006-bolt-006 | Frontend unit tests (Vitest) | intent-006 (ECOM-002) | PENDING | S | |
| 006-bolt-007 | Playwright E2E: upload → view → delete image | intent-006 (ECOM-002) | PENDING | S | |
| 007-bolt-001 | Backend ProductVariant + Inventory models + DB migration | intent-007 (ECOM-002) | PENDING | M | |
| 007-bolt-002 | Backend InventoryService (non-negative guard) | intent-007 (ECOM-002) | PENDING | S | |
| 007-bolt-003 | Backend variant + inventory endpoints | intent-007 (ECOM-002) | PENDING | S | |
| 007-bolt-004 | Backend unit tests (xUnit) | intent-007 (ECOM-002) | PENDING | S | |
| 007-bolt-005 | Frontend variant + inventory UI | intent-007 (ECOM-002) | PENDING | M | |
| 007-bolt-006 | Frontend unit tests (Vitest) | intent-007 (ECOM-002) | PENDING | S | |
| 007-bolt-007 | Playwright E2E: add variant → set inventory | intent-007 (ECOM-002) | PENDING | S | |
| 008-bolt-001 | Backend public product list/detail (Active-only) | intent-008 (ECOM-002) | PENDING | S | |
| 008-bolt-002 | Backend unit tests (xUnit) | intent-008 (ECOM-002) | PENDING | S | |
| 008-bolt-003 | Frontend product listing page | intent-008 (ECOM-002) | PENDING | S | |
| 008-bolt-004 | Frontend product detail page | intent-008 (ECOM-002) | PENDING | S | |
| 008-bolt-005 | Frontend unit tests (Vitest) | intent-008 (ECOM-002) | PENDING | S | |
| 008-bolt-006 | Playwright E2E: browse catalog → open detail | intent-008 (ECOM-002) | PENDING | S | |

---

## Completed Bolts
| ID | Bolt | Intent | Validation |
|----|------|--------|------------|
| 001-bolt-001 | Backend User + RefreshToken models + DB migration + DbContext | intent-001 (ECOM-001) | 5/5 unit tests pass, build clean |
| 001-bolt-002 | Backend AuthService (register + login + JWT access/refresh generation) | intent-001 (ECOM-001) | 9/9 unit tests pass, 14/14 total, build clean |
| 001-bolt-003 | Backend AuthController + JWT middleware config | intent-001 (ECOM-001) | 8/8 integration tests pass, 22/22 total, build clean |
| 001-bolt-004 | Backend unit tests (xUnit) | intent-001 (ECOM-001) | Satisfied by 001-bolt-002/003 tests (22/22) |
| 001-bolt-005 | Frontend auth service + types | intent-001 (ECOM-001) | 4/4 Vitest tests pass, build clean |
| 001-bolt-006 | Frontend Register page + Login page | intent-001 (ECOM-001) | 8/8 Vitest tests pass, build clean |
| 001-bolt-007 | Frontend auth context + protected route + logout | intent-001 (ECOM-001) | 13/13 Vitest tests pass, build clean |

---

## Bug Register

| Bug ID | Bolt | Severity | GitHub Issue | PR | Status |
|--------|------|----------|-------------|-----|--------|
| — | — | — | — | — | — |

---

## Instructions for Agent
- Always update this file when bolt status changes
- Never skip updating bug register when bug lifecycle triggers
- Current active bolt is the first IN PROGRESS row
- If no IN PROGRESS, pick the first PENDING bolt
