# Intent: Product Variants & Inventory

## ID
intent-007

## Source Ticket
ECOM-002 (Epic: Product Catalog Management) — https://github.com/Mana95/E-commercice-/issues/7

## Status
DRAFT

## Priority
MEDIUM

## Goal
Allow an administrator to configure product variants (e.g. size/color) and track inventory quantity per product (or per variant), ensuring quantity never goes negative.

## Affected Modules
- [x] Backend — ProductVariantService, InventoryService, ProductVariant + Inventory models
- [x] Database — ProductVariants, Inventory tables
- [x] Frontend — variant configuration UI, inventory quantity field on product edit form

## Acceptance Criteria
- [ ] AC-001: Admin can add one or more variants to a product (e.g. Size: S/M/L)
- [ ] AC-002: Each variant (or the base product, if no variants) has its own inventory quantity
- [ ] AC-003: Inventory quantity cannot be negative — rejected with 400 if attempted
- [ ] AC-004: Admin can update inventory quantity via PUT /api/v1/products/{id}/inventory
- [ ] AC-005: Inventory updates are reflected immediately in subsequent GET requests
- [ ] AC-006: Frontend shows current inventory quantity and allows editing it per variant

## Bolts

| Bolt ID | Description | Status |
|---------|-------------|--------|
| 007-bolt-001 | Backend ProductVariant + Inventory models + DB migration | PENDING |
| 007-bolt-002 | Backend InventoryService (get/update quantity, non-negative guard) | PENDING |
| 007-bolt-003 | Backend variant + inventory endpoints on ProductController | PENDING |
| 007-bolt-004 | Backend unit tests (xUnit) | PENDING |
| 007-bolt-005 | Frontend variant configuration + inventory quantity UI | PENDING |
| 007-bolt-006 | Frontend unit tests (Vitest) | PENDING |
| 007-bolt-007 | Playwright E2E: add variant → set inventory → verify immediate reflection | PENDING |

## Discovery Notes
Depends on intent-005 (Product must exist first). No existing code found. Mode 2.

## Assumptions
- Inventory is tracked at the variant level when variants exist, otherwise at the base product level
- No multi-warehouse/location inventory in this epic — single quantity value per variant/product

## Out of Scope
- Multi-warehouse inventory
- Low-stock alerts/notifications
- Product images (see intent-006)