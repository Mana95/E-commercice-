import { afterEach, describe, expect, it, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { LoginPage } from './LoginPage'
import { AuthProvider } from '../components/AuthProvider'
import * as authServiceModule from '../services/authService'
import { tokenStorage } from '../services/tokenStorage'

describe('LoginPage', () => {
  afterEach(() => {
    vi.restoreAllMocks()
    tokenStorage.clearTokens()
  })

  it('logs in and stores tokens on success', async () => {
    vi.spyOn(authServiceModule.authService, 'login').mockResolvedValue({
      userId: 'user-1',
      email: 'ada@example.com',
      accessToken: 'access-token',
      refreshToken: 'refresh-token',
    })

    render(
      <MemoryRouter>
        <AuthProvider>
          <LoginPage />
        </AuthProvider>
      </MemoryRouter>,
    )

    fireEvent.change(screen.getByLabelText('Email'), { target: { value: 'ada@example.com' } })
    fireEvent.change(screen.getByLabelText('Password'), { target: { value: 'Str0ng!Pass' } })
    fireEvent.click(screen.getByRole('button', { name: /log in/i }))

    await waitFor(() => {
      expect(tokenStorage.getAccessToken()).toBe('access-token')
      expect(tokenStorage.getRefreshToken()).toBe('refresh-token')
    })
  })

  it('shows an error message on invalid credentials', async () => {
    vi.spyOn(authServiceModule.authService, 'login').mockRejectedValue(
      Object.assign(new Error('Invalid email or password.'), { detail: 'Invalid email or password.' }),
    )

    render(
      <MemoryRouter>
        <AuthProvider>
          <LoginPage />
        </AuthProvider>
      </MemoryRouter>,
    )

    fireEvent.change(screen.getByLabelText('Email'), { target: { value: 'wrong@example.com' } })
    fireEvent.change(screen.getByLabelText('Password'), { target: { value: 'bad' } })
    fireEvent.click(screen.getByRole('button', { name: /log in/i }))

    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeInTheDocument()
    })
    expect(tokenStorage.getAccessToken()).toBeNull()
  })
})
