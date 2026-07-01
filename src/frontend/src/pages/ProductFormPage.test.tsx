import { afterEach, describe, expect, it, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter, Routes, Route } from 'react-router-dom'
import { ProductFormPage } from './ProductFormPage'
import * as productServiceModule from '../services/productService'
import * as categoryServiceModule from '../services/categoryService'

function renderAt(path: string) {
  return render(
    <MemoryRouter initialEntries={[path]}>
      <Routes>
        <Route path="/admin/products/new" element={<ProductFormPage />} />
        <Route path="/admin/products/:id" element={<ProductFormPage />} />
      </Routes>
    </MemoryRouter>,
  )
}

describe('ProductFormPage', () => {
  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('create mode submits a new product with the entered fields', async () => {
    vi.spyOn(categoryServiceModule.categoryService, 'list').mockResolvedValue([{ id: 'c1', name: 'Electronics' }])
    const createSpy = vi.spyOn(productServiceModule.productService, 'create').mockResolvedValue({
      id: 'p1', name: 'Mouse', sku: 'SKU-1', price: 10, status: 'Draft', categoryId: 'c1', brandId: null, createdAt: '', updatedAt: '',
    })

    renderAt('/admin/products/new')

    await waitFor(() => expect(screen.getByText('Electronics')).toBeInTheDocument())

    fireEvent.change(screen.getByLabelText('Name'), { target: { value: 'Mouse' } })
    fireEvent.change(screen.getByLabelText('SKU'), { target: { value: 'SKU-1' } })
    fireEvent.change(screen.getByLabelText('Price'), { target: { value: '10' } })
    fireEvent.change(screen.getByLabelText('Category'), { target: { value: 'c1' } })
    fireEvent.click(screen.getByRole('button', { name: /save/i }))

    await waitFor(() => {
      expect(createSpy).toHaveBeenCalledWith({ name: 'Mouse', sku: 'SKU-1', price: 10, categoryId: 'c1', status: 'Draft' })
    })
  })

  it('edit mode loads the existing product and submits an update', async () => {
    vi.spyOn(categoryServiceModule.categoryService, 'list').mockResolvedValue([{ id: 'c1', name: 'Electronics' }])
    vi.spyOn(productServiceModule.productService, 'getById').mockResolvedValue({
      id: 'p1', name: 'Mouse', sku: 'SKU-1', price: 10, status: 'Draft', categoryId: 'c1', brandId: null, createdAt: '', updatedAt: '',
    })
    const updateSpy = vi.spyOn(productServiceModule.productService, 'update').mockResolvedValue({
      id: 'p1', name: 'Mouse Pro', sku: 'SKU-1', price: 15, status: 'Active', categoryId: 'c1', brandId: null, createdAt: '', updatedAt: '',
    })

    renderAt('/admin/products/p1')

    await waitFor(() => expect(screen.getByDisplayValue('Mouse')).toBeInTheDocument())
    expect(screen.queryByLabelText('SKU')).not.toBeInTheDocument()

    fireEvent.change(screen.getByLabelText('Name'), { target: { value: 'Mouse Pro' } })
    fireEvent.click(screen.getByRole('button', { name: /save/i }))

    await waitFor(() => {
      expect(updateSpy).toHaveBeenCalledWith('p1', { name: 'Mouse Pro', price: 10, categoryId: 'c1', status: 'Draft' })
    })
  })

  it('edit mode archive button calls archive', async () => {
    vi.spyOn(categoryServiceModule.categoryService, 'list').mockResolvedValue([{ id: 'c1', name: 'Electronics' }])
    vi.spyOn(productServiceModule.productService, 'getById').mockResolvedValue({
      id: 'p1', name: 'Mouse', sku: 'SKU-1', price: 10, status: 'Active', categoryId: 'c1', brandId: null, createdAt: '', updatedAt: '',
    })
    const archiveSpy = vi.spyOn(productServiceModule.productService, 'archive').mockResolvedValue({
      id: 'p1', name: 'Mouse', sku: 'SKU-1', price: 10, status: 'Archived', categoryId: 'c1', brandId: null, createdAt: '', updatedAt: '',
    })

    renderAt('/admin/products/p1')

    await waitFor(() => expect(screen.getByDisplayValue('Mouse')).toBeInTheDocument())
    fireEvent.click(screen.getByRole('button', { name: /archive/i }))

    await waitFor(() => {
      expect(archiveSpy).toHaveBeenCalledWith('p1')
    })
  })

  it('shows an error message when saving fails', async () => {
    vi.spyOn(categoryServiceModule.categoryService, 'list').mockResolvedValue([{ id: 'c1', name: 'Electronics' }])
    vi.spyOn(productServiceModule.productService, 'create').mockRejectedValue(
      Object.assign(new Error('A product with this SKU already exists.'), { detail: 'A product with this SKU already exists.' }),
    )

    renderAt('/admin/products/new')

    await waitFor(() => expect(screen.getByText('Electronics')).toBeInTheDocument())

    fireEvent.change(screen.getByLabelText('Name'), { target: { value: 'Mouse' } })
    fireEvent.change(screen.getByLabelText('SKU'), { target: { value: 'SKU-1' } })
    fireEvent.change(screen.getByLabelText('Price'), { target: { value: '10' } })
    fireEvent.change(screen.getByLabelText('Category'), { target: { value: 'c1' } })
    fireEvent.click(screen.getByRole('button', { name: /save/i }))

    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeInTheDocument()
    })
  })
})
