# Intent: Customer Product Browsing

## ID
intent-008

## Source Ticket
ECOM-002 (Epic: Product Catalog Management) — https://github.com/Mana95/E-commercice-/issues/7

## Status
DRAFT

## Priority
HIGH

## Goal
Allow customers to browse the product catalog and view product details, seeing only Active products.

## Affected Modules
- [x] Backend — public product list/detail endpoints (filtered to Active status)
- [x] Frontend — product listing page, product detail page

## Acceptance Criteria
- [ ] AC-001: Customers only see products with status = Active (Draft/Archived are hidden)
- [ ] AC-002: GET /api/v1/products (public) lists active products
- [ ] AC-003: GET /api/v1/products/{id} (public) returns 404 if the product is not Active
- [ ] AC-004: Product details page displays name, price, images, description, and available variants correctly
- [ ] AC-005: Product listing page displays active products with name, price, and thumbnail image

## Bolts

| Bolt ID | Description | Status |
|---------|-------------|--------|
| 008-bolt-001 | Backend public product list/detail endpoints (Active-only filter) | PENDING |
| 008-bolt-002 | Backend unit tests (xUnit) | PENDING |
| 008-bolt-003 | Frontend product listing page | PENDING |
| 008-bolt-004 | Frontend product detail page | PENDING |
| 008-bolt-005 | Frontend unit tests (Vitest) | PENDING |
| 008-bolt-006 | Playwright E2E: browse catalog → open product detail | PENDING |

## Discovery Notes
Depends on intent-005 (Product CRUD/status) and benefits from intent-006 (images) and intent-007 (variants/inventory) for full detail display, though it can ship against intent-005 alone with images/variants added incrementally.

## Assumptions
- No search/filter/pagination requirements specified in the epic — basic list endpoint only; can be extended later if requested

## Out of Scope
- Search and filtering
- Product reviews, wishlist, shopping cart (explicitly out of scope per epic)