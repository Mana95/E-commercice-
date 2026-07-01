import { createContext, useCallback, useState, type ReactNode } from 'react'
import { authService } from '../services/authService'
import { tokenStorage } from '../services/tokenStorage'

export interface AuthContextValue {
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => Promise<void>
}

export const AuthContext = createContext<AuthContextValue | null>(null)

interface AuthProviderProps {
  children: ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [isAuthenticated, setIsAuthenticated] = useState(() => tokenStorage.getAccessToken() !== null)

  const login = useCallback(async (email: string, password: string) => {
    const result = await authService.login({ email, password })
    tokenStorage.setTokens(result.accessToken, result.refreshToken)
    setIsAuthenticated(true)
  }, [])

  const logout = useCallback(async () => {
    const refreshToken = tokenStorage.getRefreshToken()
    if (refreshToken) {
      await authService.logout(refreshToken).catch(() => {
        // best-effort server-side revoke; proceed with local logout regardless
      })
    }
    tokenStorage.clearTokens()
    setIsAuthenticated(false)
  }, [])

  return (
    <AuthContext.Provider value={{ isAuthenticated, login, logout }}>{children}</AuthContext.Provider>
  )
}
