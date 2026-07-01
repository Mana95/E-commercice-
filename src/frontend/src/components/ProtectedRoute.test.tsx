import { afterEach, describe, expect, it } from 'vitest'
import { render, screen } from '@testing-library/react'
import { MemoryRouter, Routes, Route } from 'react-router-dom'
import { ProtectedRoute } from './ProtectedRoute'
import { AuthProvider } from './AuthProvider'
import { tokenStorage } from '../services/tokenStorage'

function renderProtected() {
  return render(
    <MemoryRouter initialEntries={['/']}>
      <AuthProvider>
        <Routes>
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <p>secret content</p>
              </ProtectedRoute>
            }
          />
          <Route path="/login" element={<p>login page</p>} />
        </Routes>
      </AuthProvider>
    </MemoryRouter>,
  )
}

describe('ProtectedRoute', () => {
  afterEach(() => {
    tokenStorage.clearTokens()
  })

  it('redirects to /login when not authenticated', () => {
    renderProtected()

    expect(screen.getByText('login page')).toBeInTheDocument()
    expect(screen.queryByText('secret content')).not.toBeInTheDocument()
  })

  it('renders children when authenticated', () => {
    tokenStorage.setTokens('access-token', 'refresh-token')

    renderProtected()

    expect(screen.getByText('secret content')).toBeInTheDocument()
  })
})
