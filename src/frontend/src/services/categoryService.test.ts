import { afterEach, describe, expect, it, vi } from 'vitest'
import { categoryService } from './categoryService'

describe('categoryService', () => {
  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('list fetches GET /categories and returns the parsed array', async () => {
    const mockCategories = [{ id: 'c1', name: 'Electronics' }]
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({ ok: true, status: 200, json: async () => mockCategories }),
    )

    const result = await categoryService.list()

    expect(result).toEqual(mockCategories)
    expect(fetch).toHaveBeenCalledWith(expect.stringContaining('/categories'), expect.anything())
  })
})
