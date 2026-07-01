import { apiFetch } from './apiClient'
import type { Category } from '../types/Category'

export const categoryService = {
  list(): Promise<Category[]> {
    return apiFetch<Category[]>('/categories')
  },
}
