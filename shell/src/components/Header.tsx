import { useLocation } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

const BREADCRUMBS: Record<string, { parent?: string; title: string }> = {
  '/':               { title: 'Visão Geral' },
  '/business-units': { parent: 'Módulos', title: 'Empresas' },
  '/users':          { parent: 'Módulos', title: 'Usuários' },
}

function getBreadcrumb(pathname: string) {
  for (const [path, crumb] of Object.entries(BREADCRUMBS)) {
    if (pathname === path || (path !== '/' && pathname.startsWith(path))) return crumb
  }
  return { title: 'Platform' }
}

function IconBell() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.75" strokeLinecap="round" strokeLinejoin="round" className="w-5 h-5">
      <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
      <path d="M13.73 21a2 2 0 0 1-3.46 0" />
    </svg>
  )
}

function IconLogout() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.75" strokeLinecap="round" strokeLinejoin="round" className="w-4 h-4">
      <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
      <polyline points="16 17 21 12 16 7" />
      <line x1="21" y1="12" x2="9" y2="12" />
    </svg>
  )
}

export function Header() {
  const { pathname } = useLocation()
  const { user, logout } = useAuth()
  const crumb = getBreadcrumb(pathname)

  return (
    <header className="flex h-16 shrink-0 items-center justify-between border-b border-neutral-200 bg-white px-6 z-10">
      {/* Breadcrumb */}
      <div className="flex items-center gap-2 text-sm">
        {crumb.parent && (
          <>
            <span className="text-neutral-400">{crumb.parent}</span>
            <svg viewBox="0 0 24 24" className="w-3.5 h-3.5 text-neutral-300" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
              <polyline points="9 18 15 12 9 6" />
            </svg>
          </>
        )}
        <span className="font-semibold text-neutral-900">{crumb.title}</span>
      </div>

      {/* Actions */}
      <div className="flex items-center gap-2">
        <button className="relative flex h-9 w-9 items-center justify-center rounded-xl text-neutral-500 transition-colors hover:bg-neutral-100">
          <IconBell />
          <span className="absolute top-2 right-2 flex h-2 w-2 rounded-full bg-danger-500 ring-2 ring-white" />
        </button>

        <div className="mx-1 h-6 w-px bg-neutral-200" />

        {user && (
          <div className="flex items-center gap-2.5">
            <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-primary-100 text-primary-600 text-sm font-bold select-none shrink-0">
              {user.name.charAt(0).toUpperCase()}
            </div>
            <div className="hidden sm:block leading-tight">
              <p className="text-sm font-semibold text-neutral-900">{user.name}</p>
              <p className="text-xs text-neutral-400">{user.email}</p>
            </div>
            <button
              onClick={logout}
              title="Sair da conta"
              className="ml-1 flex h-9 w-9 items-center justify-center rounded-xl text-neutral-400 transition-colors hover:bg-neutral-100 hover:text-danger-600"
            >
              <IconLogout />
            </button>
          </div>
        )}
      </div>
    </header>
  )
}
