import { afterEach, describe, expect, it, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { ProductListPage } from './ProductListPage'
import * as productServiceModule from '../services/productService'

describe('ProductListPage', () => {
  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('shows a loading state, then renders the product table', async () => {
    vi.spyOn(productServiceModule.productService, 'list').mockResolvedValue([
      { id: '1', name: 'Wireless Mouse', sku: 'SKU-1', price: 29.99, status: 'Active', categoryId: 'c1', brandId: null, createdAt: '', updatedAt: '' },
    ])

    render(
      <MemoryRouter>
        <ProductListPage />
      </MemoryRouter>,
    )

    expect(screen.getByText(/loading products/i)).toBeInTheDocument()

    await waitFor(() => {
      expect(screen.getByText('Wireless Mouse')).toBeInTheDocument()
    })
    expect(screen.getByText('SKU-1')).toBeInTheDocument()
    expect(screen.getByText('$29.99')).toBeInTheDocument()
    expect(screen.getByText('Active')).toBeInTheDocument()
  })

  it('shows an empty state when there are no products', async () => {
    vi.spyOn(productServiceModule.productService, 'list').mockResolvedValue([])

    render(
      <MemoryRouter>
        <ProductListPage />
      </MemoryRouter>,
    )

    await waitFor(() => {
      expect(screen.getByText(/no products yet/i)).toBeInTheDocument()
    })
  })

  it('shows an error message when the fetch fails', async () => {
    vi.spyOn(productServiceModule.productService, 'list').mockRejectedValue(
      Object.assign(new Error('Failed to load products.'), { detail: 'Failed to load products.' }),
    )

    render(
      <MemoryRouter>
        <ProductListPage />
      </MemoryRouter>,
    )

    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeInTheDocument()
    })
  })
})
