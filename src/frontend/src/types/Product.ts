export type ProductStatus = 'Draft' | 'Active' | 'Archived'

export interface Product {
  id: string
  name: string
  sku: string
  price: number
  status: ProductStatus
  categoryId: string
  brandId: string | null
  createdAt: string
  updatedAt: string
}

export interface CreateProductRequest {
  name: string
  sku: string
  price: number
  categoryId: string
  brandId?: string | null
  status?: ProductStatus
}

export interface UpdateProductRequest {
  name: string
  price: number
  categoryId: string
  brandId?: string | null
  status: ProductStatus
}
