import { apiFetch } from './apiClient'
import type { AuthResponse, LoginRequest, RegisterRequest } from '../types/Auth'

export const authService = {
  register(request: RegisterRequest): Promise<AuthResponse> {
    return apiFetch<AuthResponse>('/auth/register', {
      method: 'POST',
      body: JSON.stringify(request),
    })
  },

  login(request: LoginRequest): Promise<AuthResponse> {
    return apiFetch<AuthResponse>('/auth/login', {
      method: 'POST',
      body: JSON.stringify(request),
    })
  },

  logout(refreshToken: string): Promise<void> {
    return apiFetch<void>('/auth/logout', {
      method: 'POST',
      body: JSON.stringify({ refreshToken }),
    })
  },

  refreshToken(refreshToken: string): Promise<AuthResponse> {
    return apiFetch<AuthResponse>('/auth/refresh-token', {
      method: 'POST',
      body: JSON.stringify({ refreshToken }),
    })
  },
}
