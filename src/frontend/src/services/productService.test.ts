import { afterEach, describe, expect, it, vi } from 'vitest'
import { productService } from './productService'

describe('productService', () => {
  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('list fetches GET /products and returns the parsed array', async () => {
    const mockProducts = [
      { id: '1', name: 'Mouse', sku: 'SKU-1', price: 10, status: 'Active', categoryId: 'c1', brandId: null, createdAt: '', updatedAt: '' },
    ]
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({ ok: true, status: 200, json: async () => mockProducts }),
    )

    const result = await productService.list()

    expect(result).toEqual(mockProducts)
    expect(fetch).toHaveBeenCalledWith(expect.stringContaining('/products'), expect.anything())
  })

  it('create sends POST with the request body', async () => {
    const mockProduct = { id: '1', name: 'Mouse', sku: 'SKU-1', price: 10, status: 'Draft', categoryId: 'c1', brandId: null, createdAt: '', updatedAt: '' }
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({ ok: true, status: 201, json: async () => mockProduct }),
    )

    const result = await productService.create({ name: 'Mouse', sku: 'SKU-1', price: 10, categoryId: 'c1' })

    expect(result).toEqual(mockProduct)
    expect(fetch).toHaveBeenCalledWith(expect.stringContaining('/products'), expect.objectContaining({ method: 'POST' }))
  })

  it('archive sends DELETE to /products/:id', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({ ok: true, status: 200, json: async () => ({}) }),
    )

    await productService.archive('1')

    expect(fetch).toHaveBeenCalledWith(expect.stringContaining('/products/1'), expect.objectContaining({ method: 'DELETE' }))
  })
})
