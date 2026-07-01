import type { ApiErrorResponse } from '../types/Auth'

export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '/api/v1'

export class ApiError extends Error {
  status: number
  detail?: string

  constructor(status: number, message: string, detail?: string) {
    super(message)
    this.status = status
    this.detail = detail
  }
}

export async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
  })

  if (!response.ok) {
    let body: ApiErrorResponse = {}
    try {
      body = await response.json()
    } catch {
      // response had no JSON body
    }
    throw new ApiError(response.status, body.title ?? 'Request failed', body.detail)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}
