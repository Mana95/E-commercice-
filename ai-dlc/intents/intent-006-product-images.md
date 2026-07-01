# Intent: Product Images

## ID
intent-006

## Source Ticket
ECOM-002 (Epic: Product Catalog Management) — https://github.com/Mana95/E-commercice-/issues/7

## Status
DRAFT

## Priority
MEDIUM

## Goal
Allow an administrator to upload and manage multiple images per product.

## Affected Modules
- [x] Backend — ProductImageService, ProductImage model, file storage integration
- [x] Database — ProductImages table
- [x] Frontend — image upload UI on product edit form, image gallery

## Acceptance Criteria
- [ ] AC-001: Admin can upload one or more images to a product via POST /api/v1/products/{id}/images
- [ ] AC-002: Each image is linked to its product and stored with an ordering/position
- [ ] AC-003: Admin can delete an individual product image
- [ ] AC-004: Invalid file types/oversized files are rejected with 400
- [ ] AC-005: Frontend shows uploaded images in a gallery on the product edit page and supports removing one

## Bolts

| Bolt ID | Description | Status |
|---------|-------------|--------|
| 006-bolt-001 | Backend ProductImage model + DB migration | PENDING |
| 006-bolt-002 | Backend ProductImageService (upload/validate/delete) | PENDING |
| 006-bolt-003 | Backend image endpoints on ProductController | PENDING |
| 006-bolt-004 | Backend unit tests (xUnit) | PENDING |
| 006-bolt-005 | Frontend image upload + gallery UI | PENDING |
| 006-bolt-006 | Frontend unit tests (Vitest) | PENDING |
| 006-bolt-007 | Playwright E2E: upload images → view gallery → delete image | PENDING |

## Discovery Notes
Depends on intent-005 (Product must exist first). No file storage provider configured — blocking issue, see Open Question below.

## Open Question for User
This intent requires persisting uploaded image files somewhere durable (cloud blob storage, local disk + served path, or a 3rd-party media service). No such provider is in the locked tech stack, and `architecture.md` explicitly requires asking before introducing cloud-specific code. **Cannot start building past the ProductImage model until this is decided.**

## Assumptions
- Once a storage provider is chosen, ProductImage stores a URL/path + position + productId, not the binary itself in the database

## Out of Scope
- Image cropping/editing tools
- CDN/image optimization pipeline