import { useState, type FormEvent } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { authService } from '../services/authService'
import { ApiError } from '../services/apiClient'

export function RegisterPage() {
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setIsSubmitting(true)

    try {
      await authService.register({ email, password, firstName, lastName })
      navigate('/login')
    } catch (err) {
      setError(err instanceof ApiError ? (err.detail ?? err.message) : 'Registration failed.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center px-4">
      <form onSubmit={handleSubmit} className="w-full max-w-sm space-y-4" aria-label="Register">
        <h1 className="text-2xl font-semibold">Create account</h1>

        {error && (
          <p role="alert" className="text-red-600 text-sm">
            {error}
          </p>
        )}

        <div>
          <label htmlFor="firstName" className="block text-sm mb-1">
            First name
          </label>
          <input
            id="firstName"
            className="w-full border rounded px-3 py-2"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            required
          />
        </div>

        <div>
          <label htmlFor="lastName" className="block text-sm mb-1">
            Last name
          </label>
          <input
            id="lastName"
            className="w-full border rounded px-3 py-2"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            required
          />
        </div>

        <div>
          <label htmlFor="email" className="block text-sm mb-1">
            Email
          </label>
          <input
            id="email"
            type="email"
            className="w-full border rounded px-3 py-2"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>

        <div>
          <label htmlFor="password" className="block text-sm mb-1">
            Password
          </label>
          <input
            id="password"
            type="password"
            className="w-full border rounded px-3 py-2"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>

        <button
          type="submit"
          disabled={isSubmitting}
          className="w-full bg-black text-white rounded py-2 disabled:opacity-50"
        >
          {isSubmitting ? 'Creating account…' : 'Create account'}
        </button>

        <p className="text-sm text-center">
          Already have an account? <Link to="/login">Log in</Link>
        </p>
      </form>
    </div>
  )
}
