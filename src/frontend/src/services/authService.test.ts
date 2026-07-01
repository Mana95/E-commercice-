import { afterEach, describe, expect, it, vi } from 'vitest'
import { authService } from './authService'

describe('authService', () => {
  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('register sends POST to /auth/register and returns the parsed response', async () => {
    const mockResponse = {
      userId: 'user-1',
      email: 'customer@example.com',
      accessToken: 'access-token',
      refreshToken: 'refresh-token',
    }
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({
        ok: true,
        status: 201,
        json: async () => mockResponse,
      }),
    )

    const result = await authService.register({
      email: 'customer@example.com',
      password: 'Str0ng!Pass',
      firstName: 'Ada',
      lastName: 'Lovelace',
    })

    expect(result).toEqual(mockResponse)
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/auth/register'),
      expect.objectContaining({ method: 'POST' }),
    )
  })

  it('login throws ApiError with status and detail on 401', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({
        ok: false,
        status: 401,
        json: async () => ({ title: 'Login failed', detail: 'Invalid email or password.' }),
      }),
    )

    await expect(
      authService.login({ email: 'wrong@example.com', password: 'bad' }),
    ).rejects.toMatchObject({
      status: 401,
      detail: 'Invalid email or password.',
    })
  })

  it('logout sends POST with refresh token in body', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({ ok: true, status: 204, json: async () => undefined }),
    )

    await authService.logout('some-refresh-token')

    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/auth/logout'),
      expect.objectContaining({ body: JSON.stringify({ refreshToken: 'some-refresh-token' }) }),
    )
  })

  it('refreshToken returns new tokens on success', async () => {
    const mockResponse = {
      userId: 'user-1',
      email: 'customer@example.com',
      accessToken: 'new-access-token',
      refreshToken: 'new-refresh-token',
    }
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({ ok: true, status: 200, json: async () => mockResponse }),
    )

    const result = await authService.refreshToken('old-refresh-token')

    expect(result).toEqual(mockResponse)
  })
})
