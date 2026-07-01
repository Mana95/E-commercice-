# Intent: Product Catalog — CRUD, Pricing, Status

## ID
intent-005

## Source Ticket
ECOM-002 (Epic: Product Catalog Management) — https://github.com/Mana95/E-commercice-/issues/7

## Status
DRAFT

## Priority
HIGH

## Goal
Allow an administrator to create, update, delete/archive, and view products, with unique SKU, valid pricing, and status management (Draft/Active/Archived).

## Affected Modules
- [x] Backend — ProductController, ProductService, Product model, Category/Brand models
- [x] Database — Products, Categories, Brands tables
- [x] Frontend — Admin product list, create/edit product form

## Acceptance Criteria
- [ ] AC-001: Admin can create a new product with name, SKU, price, category, status
- [ ] AC-002: SKU must be unique; duplicate SKU returns 409
- [ ] AC-003: Product cannot be saved without required fields (name, SKU, price, quantity) — 400 with validation errors
- [ ] AC-004: Price must be greater than zero — rejected otherwise
- [ ] AC-005: Admin can update an existing product
- [ ] AC-006: Admin can archive a product (status → Archived); archived products are not hard-deleted
- [ ] AC-007: Product belongs to at least one category
- [ ] AC-008: GET product by id returns full product details
- [ ] AC-009: Unauthenticated requests to admin product endpoints return 401 (uses JWT from intent-001)
- [ ] AC-010: Frontend admin list shows all products regardless of status; create/edit form validates and shows API errors

## Bolts

| Bolt ID | Description | Status |
|---------|-------------|--------|
| 005-bolt-001 | Backend Product + Category + Brand models + DB migration + DbContext update | PENDING |
| 005-bolt-002 | Backend ProductService (create/update/archive/get/list, SKU uniqueness, price validation) | PENDING |
| 005-bolt-003 | Backend ProductController (`/api/v1/products` CRUD) + `[Authorize]` guard | PENDING |
| 005-bolt-004 | Backend unit tests (xUnit) | PENDING |
| 005-bolt-005 | Frontend admin product list page | PENDING |
| 005-bolt-006 | Frontend admin create/edit product form | PENDING |
| 005-bolt-007 | Frontend unit tests (Vitest) | PENDING |
| 005-bolt-008 | Playwright E2E: admin creates → edits → archives a product | PENDING |

## Discovery Notes
No existing product/catalog code found. Building from scratch (Mode 2). Depends on intent-001 (JWT auth) for admin-only endpoints.

## Assumptions
- "Administrator" role reuses the JWT auth from intent-001; role-based authorization itself was flagged as a future intent in intent-001 — for now, any authenticated user is treated as able to manage products (revisit once roles/authorization intent exists)
- Archiving is a soft state change (status = Archived), not a hard delete, per business rule "Products can be Active, Draft, or Archived"
- Category and Brand are simple reference entities (name only) for this intent; deeper category management is not in this epic's scope

## Out of Scope
- Product images (see intent-006)
- Variants and inventory quantity (see intent-007)
- Customer-facing browsing/listing (see intent-008)
- Product reviews, wishlist, shopping cart (explicitly out of scope per epic)
- Role-based authorization (still pending from intent-001)