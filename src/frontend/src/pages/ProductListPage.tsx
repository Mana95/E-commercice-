import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { productService } from '../services/productService'
import { ApiError } from '../services/apiClient'
import type { Product } from '../types/Product'

export function ProductListPage() {
  const [products, setProducts] = useState<Product[] | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false

    productService
      .list()
      .then((result) => {
        if (!cancelled) setProducts(result)
      })
      .catch((err) => {
        if (!cancelled) {
          setError(err instanceof ApiError ? (err.detail ?? err.message) : 'Failed to load products.')
        }
      })

    return () => {
      cancelled = true
    }
  }, [])

  if (error) {
    return (
      <div className="p-6">
        <p role="alert" className="text-red-600">
          {error}
        </p>
      </div>
    )
  }

  if (products === null) {
    return (
      <div className="p-6">
        <p>Loading products…</p>
      </div>
    )
  }

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-4">
        <h1 className="text-2xl font-semibold">Products</h1>
        <Link to="/admin/products/new" className="bg-black text-white rounded px-4 py-2">
          New Product
        </Link>
      </div>

      {products.length === 0 ? (
        <p>No products yet.</p>
      ) : (
        <table className="w-full text-left border-collapse">
          <thead>
            <tr className="border-b">
              <th className="py-2">Name</th>
              <th className="py-2">SKU</th>
              <th className="py-2">Price</th>
              <th className="py-2">Status</th>
            </tr>
          </thead>
          <tbody>
            {products.map((product) => (
              <tr key={product.id} className="border-b">
                <td className="py-2">
                  <Link to={`/admin/products/${product.id}`}>{product.name}</Link>
                </td>
                <td className="py-2">{product.sku}</td>
                <td className="py-2">${product.price.toFixed(2)}</td>
                <td className="py-2">{product.status}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
