import { useEffect, useState, type FormEvent } from 'react'
import { useNavigate, useParams, Link } from 'react-router-dom'
import { productService } from '../services/productService'
import { categoryService } from '../services/categoryService'
import { ApiError } from '../services/apiClient'
import type { Category } from '../types/Category'
import type { ProductStatus } from '../types/Product'

export function ProductFormPage() {
  const navigate = useNavigate()
  const { id } = useParams<{ id: string }>()
  const isEditMode = Boolean(id)

  const [categories, setCategories] = useState<Category[]>([])
  const [name, setName] = useState('')
  const [sku, setSku] = useState('')
  const [price, setPrice] = useState('')
  const [categoryId, setCategoryId] = useState('')
  const [status, setStatus] = useState<ProductStatus>('Draft')
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(isEditMode)
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    categoryService.list().then(setCategories).catch(() => setCategories([]))
  }, [])

  useEffect(() => {
    if (!id) return

    productService
      .getById(id)
      .then((product) => {
        setName(product.name)
        setSku(product.sku)
        setPrice(String(product.price))
        setCategoryId(product.categoryId)
        setStatus(product.status)
      })
      .catch((err) => {
        setError(err instanceof ApiError ? (err.detail ?? err.message) : 'Failed to load product.')
      })
      .finally(() => setIsLoading(false))
  }, [id])

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setIsSubmitting(true)

    try {
      if (isEditMode && id) {
        await productService.update(id, { name, price: Number(price), categoryId, status })
      } else {
        await productService.create({ name, sku, price: Number(price), categoryId, status })
      }
      navigate('/admin/products')
    } catch (err) {
      setError(err instanceof ApiError ? (err.detail ?? err.message) : 'Failed to save product.')
    } finally {
      setIsSubmitting(false)
    }
  }

  async function handleArchive() {
    if (!id) return
    setError(null)
    try {
      await productService.archive(id)
      navigate('/admin/products')
    } catch (err) {
      setError(err instanceof ApiError ? (err.detail ?? err.message) : 'Failed to archive product.')
    }
  }

  if (isLoading) {
    return (
      <div className="p-6">
        <p>Loading product…</p>
      </div>
    )
  }

  return (
    <div className="p-6 max-w-md">
      <h1 className="text-2xl font-semibold mb-4">{isEditMode ? 'Edit Product' : 'New Product'}</h1>

      <form onSubmit={handleSubmit} className="space-y-4" aria-label={isEditMode ? 'Edit Product' : 'New Product'}>
        {error && (
          <p role="alert" className="text-red-600 text-sm">
            {error}
          </p>
        )}

        <div>
          <label htmlFor="name" className="block text-sm mb-1">
            Name
          </label>
          <input
            id="name"
            className="w-full border rounded px-3 py-2"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
          />
        </div>

        {!isEditMode && (
          <div>
            <label htmlFor="sku" className="block text-sm mb-1">
              SKU
            </label>
            <input
              id="sku"
              className="w-full border rounded px-3 py-2"
              value={sku}
              onChange={(e) => setSku(e.target.value)}
              required
            />
          </div>
        )}

        <div>
          <label htmlFor="price" className="block text-sm mb-1">
            Price
          </label>
          <input
            id="price"
            type="number"
            step="0.01"
            min="0.01"
            className="w-full border rounded px-3 py-2"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
            required
          />
        </div>

        <div>
          <label htmlFor="category" className="block text-sm mb-1">
            Category
          </label>
          <select
            id="category"
            className="w-full border rounded px-3 py-2"
            value={categoryId}
            onChange={(e) => setCategoryId(e.target.value)}
            required
          >
            <option value="" disabled>
              Select a category
            </option>
            {categories.map((category) => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label htmlFor="status" className="block text-sm mb-1">
            Status
          </label>
          <select
            id="status"
            className="w-full border rounded px-3 py-2"
            value={status}
            onChange={(e) => setStatus(e.target.value as ProductStatus)}
          >
            <option value="Draft">Draft</option>
            <option value="Active">Active</option>
            <option value="Archived">Archived</option>
          </select>
        </div>

        <div className="flex gap-2">
          <button
            type="submit"
            disabled={isSubmitting}
            className="bg-black text-white rounded px-4 py-2 disabled:opacity-50"
          >
            {isSubmitting ? 'Saving…' : 'Save'}
          </button>
          {isEditMode && (
            <button type="button" onClick={handleArchive} className="border rounded px-4 py-2">
              Archive
            </button>
          )}
          <Link to="/admin/products" className="border rounded px-4 py-2">
            Cancel
          </Link>
        </div>
      </form>
    </div>
  )
}
