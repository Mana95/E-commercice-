export interface RegisterRequest {
  email: string
  password: string
  firstName: string
  lastName: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface AuthResponse {
  userId: string
  email: string
  accessToken: string
  refreshToken: string
}

export interface ApiErrorResponse {
  title?: string
  detail?: string
  status?: number
}
