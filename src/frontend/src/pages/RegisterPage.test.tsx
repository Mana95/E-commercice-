import { afterEach, describe, expect, it, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { RegisterPage } from './RegisterPage'
import * as authServiceModule from '../services/authService'

describe('RegisterPage', () => {
  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('submits registration data and navigates to login on success', async () => {
    const registerSpy = vi.spyOn(authServiceModule.authService, 'register').mockResolvedValue({
      userId: 'user-1',
      email: 'ada@example.com',
      accessToken: 'a',
      refreshToken: 'r',
    })

    render(
      <MemoryRouter>
        <RegisterPage />
      </MemoryRouter>,
    )

    fireEvent.change(screen.getByLabelText('First name'), { target: { value: 'Ada' } })
    fireEvent.change(screen.getByLabelText('Last name'), { target: { value: 'Lovelace' } })
    fireEvent.change(screen.getByLabelText('Email'), { target: { value: 'ada@example.com' } })
    fireEvent.change(screen.getByLabelText('Password'), { target: { value: 'Str0ng!Pass' } })
    fireEvent.click(screen.getByRole('button', { name: /create account/i }))

    await waitFor(() => {
      expect(registerSpy).toHaveBeenCalledWith({
        email: 'ada@example.com',
        password: 'Str0ng!Pass',
        firstName: 'Ada',
        lastName: 'Lovelace',
      })
    })
  })

  it('shows an error message when registration fails', async () => {
    vi.spyOn(authServiceModule.authService, 'register').mockRejectedValue(
      Object.assign(new Error('Email is already registered.'), { detail: 'Email is already registered.' }),
    )

    render(
      <MemoryRouter>
        <RegisterPage />
      </MemoryRouter>,
    )

    fireEvent.change(screen.getByLabelText('First name'), { target: { value: 'Ada' } })
    fireEvent.change(screen.getByLabelText('Last name'), { target: { value: 'Lovelace' } })
    fireEvent.change(screen.getByLabelText('Email'), { target: { value: 'dupe@example.com' } })
    fireEvent.change(screen.getByLabelText('Password'), { target: { value: 'Str0ng!Pass' } })
    fireEvent.click(screen.getByRole('button', { name: /create account/i }))

    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeInTheDocument()
    })
  })
})
