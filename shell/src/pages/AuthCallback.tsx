import { useEffect, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

export function AuthCallback() {
  const { handleCallback } = useAuth()
  const navigate = useNavigate()
  const handled = useRef(false)

  useEffect(() => {
    if (handled.current) return
    handled.current = true

    const params = new URLSearchParams(window.location.search)
    const code = params.get('code')
    const state = params.get('state')
    const error = params.get('error')

    if (error) {
      navigate('/login', { replace: true })
      return
    }

    if (!code || !state) {
      navigate('/login', { replace: true })
      return
    }

    handleCallback(code, state)
      .then(() => {
        const redirect = sessionStorage.getItem('redirect_after_login') ?? '/'
        sessionStorage.removeItem('redirect_after_login')
        navigate(redirect, { replace: true })
      })
      .catch(() => navigate('/login', { replace: true }))
  }, [handleCallback, navigate])

  return (
    <div className="flex h-screen items-center justify-center bg-neutral-800">
      <div className="flex flex-col items-center gap-4">
        <div className="flex h-16 w-16 items-center justify-center rounded-2xl bg-primary-600 shadow-lg">
          <svg viewBox="0 0 24 24" className="w-8 h-8" fill="none" stroke="white" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
            <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2" />
          </svg>
        </div>
        <div className="text-center">
          <p className="text-base font-semibold text-white">Autenticando</p>
          <p className="mt-1 text-sm text-neutral-400">Verificando suas credenciais...</p>
        </div>
        <div className="flex gap-1">
          <span className="h-2 w-2 rounded-full bg-primary-500 animate-bounce" style={{ animationDelay: '0ms' }} />
          <span className="h-2 w-2 rounded-full bg-primary-500 animate-bounce" style={{ animationDelay: '150ms' }} />
          <span className="h-2 w-2 rounded-full bg-primary-500 animate-bounce" style={{ animationDelay: '300ms' }} />
        </div>
      </div>
    </div>
  )
}
