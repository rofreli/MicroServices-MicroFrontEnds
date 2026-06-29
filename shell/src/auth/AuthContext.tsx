import { createContext, useCallback, useContext, useEffect, useState, type ReactNode } from 'react'
import { generateCodeChallenge, generateCodeVerifier, generateState } from './pkce'

const OAUTH_BASE = import.meta.env.VITE_OAUTH_URL ?? 'http://localhost:5001'
const CLIENT_ID = 'spa'
const REDIRECT_URI = `${window.location.origin}/auth/callback`
const SCOPES = 'openid profile email roles api'

export interface AuthUser {
  sub: string
  email: string
  name: string
  given_name: string
  family_name: string
  roles: string[]
  isActive: boolean
}

interface AuthState {
  user: AuthUser | null
  accessToken: string | null
  isLoading: boolean
  isAuthenticated: boolean
}

interface AuthContextValue extends AuthState {
  login: () => Promise<void>
  logout: () => void
  handleCallback: (code: string, state: string) => Promise<void>
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>({
    user: null,
    accessToken: localStorage.getItem('access_token'),
    isLoading: true,
    isAuthenticated: false,
  })

  // On mount: try to load existing token and fetch user info
  useEffect(() => {
    const token = localStorage.getItem('access_token')
    if (!token) {
      setState(s => ({ ...s, isLoading: false }))
      return
    }
    fetchUserInfo(token)
      .then(user => setState({ user, accessToken: token, isLoading: false, isAuthenticated: true }))
      .catch(() => {
        localStorage.removeItem('access_token')
        setState({ user: null, accessToken: null, isLoading: false, isAuthenticated: false })
      })
  }, [])

  const login = useCallback(async () => {
    const verifier = generateCodeVerifier()
    const challenge = await generateCodeChallenge(verifier)
    const state = generateState()

    sessionStorage.setItem('pkce_verifier', verifier)
    sessionStorage.setItem('oauth_state', state)
    sessionStorage.setItem('redirect_after_login', window.location.pathname)

    const params = new URLSearchParams({
      response_type: 'code',
      client_id: CLIENT_ID,
      redirect_uri: REDIRECT_URI,
      scope: SCOPES,
      state,
      code_challenge: challenge,
      code_challenge_method: 'S256',
    })

    window.location.href = `${OAUTH_BASE}/connect/authorize?${params}`
  }, [])

  const handleCallback = useCallback(async (code: string, returnedState: string) => {
    const storedState = sessionStorage.getItem('oauth_state')
    const verifier = sessionStorage.getItem('pkce_verifier')

    if (returnedState !== storedState) throw new Error('State mismatch — possible CSRF.')
    if (!verifier) throw new Error('No PKCE verifier found.')

    sessionStorage.removeItem('oauth_state')
    sessionStorage.removeItem('pkce_verifier')

    const body = new URLSearchParams({
      grant_type: 'authorization_code',
      client_id: CLIENT_ID,
      redirect_uri: REDIRECT_URI,
      code,
      code_verifier: verifier,
    })

    const res = await fetch(`${OAUTH_BASE}/connect/token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body,
    })

    if (!res.ok) {
      const err = await res.json()
      throw new Error(err.error_description ?? 'Token exchange failed.')
    }

    const tokens = await res.json()
    localStorage.setItem('access_token', tokens.access_token)
    if (tokens.refresh_token) localStorage.setItem('refresh_token', tokens.refresh_token)

    const user = await fetchUserInfo(tokens.access_token)
    setState({ user, accessToken: tokens.access_token, isLoading: false, isAuthenticated: true })
  }, [])

  const logout = useCallback(() => {
    localStorage.removeItem('access_token')
    localStorage.removeItem('refresh_token')
    setState({ user: null, accessToken: null, isLoading: false, isAuthenticated: false })

    const params = new URLSearchParams({
      client_id: CLIENT_ID,
      post_logout_redirect_uri: window.location.origin,
    })
    window.location.href = `${OAUTH_BASE}/connect/logout?${params}`
  }, [])

  return (
    <AuthContext.Provider value={{ ...state, login, logout, handleCallback }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider')
  return ctx
}

async function fetchUserInfo(accessToken: string): Promise<AuthUser> {
  const res = await fetch(`${OAUTH_BASE}/connect/userinfo`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  })
  if (!res.ok) throw new Error('Failed to fetch user info.')
  return res.json()
}
