import { afterEach, describe, expect, it, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { AuthProvider } from './AuthProvider'
import { useAuth } from '../hooks/useAuth'
import * as authServiceModule from '../services/authService'
import { tokenStorage } from '../services/tokenStorage'

function TestConsumer() {
  const { isAuthenticated, login, logout } = useAuth()
  return (
    <div>
      <p>{isAuthenticated ? 'authenticated' : 'anonymous'}</p>
      <button onClick={() => login('ada@example.com', 'Str0ng!Pass')}>login</button>
      <button onClick={() => logout()}>logout</button>
    </div>
  )
}

describe('AuthProvider', () => {
  afterEach(() => {
    vi.restoreAllMocks()
    tokenStorage.clearTokens()
  })

  it('starts unauthenticated when no tokens are stored', () => {
    render(
      <AuthProvider>
        <TestConsumer />
      </AuthProvider>,
    )

    expect(screen.getByText('anonymous')).toBeInTheDocument()
  })

  it('becomes authenticated after login() stores tokens', async () => {
    vi.spyOn(authServiceModule.authService, 'login').mockResolvedValue({
      userId: 'user-1',
      email: 'ada@example.com',
      accessToken: 'access-token',
      refreshToken: 'refresh-token',
    })

    render(
      <AuthProvider>
        <TestConsumer />
      </AuthProvider>,
    )

    screen.getByText('login').click()

    await waitFor(() => expect(screen.getByText('authenticated')).toBeInTheDocument())
    expect(tokenStorage.getAccessToken()).toBe('access-token')
  })

  it('becomes anonymous and clears tokens after logout()', async () => {
    vi.spyOn(authServiceModule.authService, 'login').mockResolvedValue({
      userId: 'user-1',
      email: 'ada@example.com',
      accessToken: 'access-token',
      refreshToken: 'refresh-token',
    })
    vi.spyOn(authServiceModule.authService, 'logout').mockResolvedValue(undefined)

    render(
      <AuthProvider>
        <TestConsumer />
      </AuthProvider>,
    )

    screen.getByText('login').click()
    await waitFor(() => expect(screen.getByText('authenticated')).toBeInTheDocument())

    screen.getByText('logout').click()

    await waitFor(() => expect(screen.getByText('anonymous')).toBeInTheDocument())
    expect(tokenStorage.getAccessToken()).toBeNull()
  })
})
