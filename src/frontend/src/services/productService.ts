import { apiFetch } from './apiClient'
import type { CreateProductRequest, Product, UpdateProductRequest } from '../types/Product'

export const productService = {
  list(): Promise<Product[]> {
    return apiFetch<Product[]>('/products')
  },

  getById(id: string): Promise<Product> {
    return apiFetch<Product>(`/products/${id}`)
  },

  create(request: CreateProductRequest): Promise<Product> {
    return apiFetch<Product>('/products', {
      method: 'POST',
      body: JSON.stringify(request),
    })
  },

  update(id: string, request: UpdateProductRequest): Promise<Product> {
    return apiFetch<Product>(`/products/${id}`, {
      method: 'PUT',
      body: JSON.stringify(request),
    })
  },

  archive(id: string): Promise<Product> {
    return apiFetch<Product>(`/products/${id}`, {
      method: 'DELETE',
    })
  },
}
