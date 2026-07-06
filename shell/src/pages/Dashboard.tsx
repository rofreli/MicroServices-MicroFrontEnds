import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

// The BFF gateway is the single entry point (same host as the OIDC endpoints).
const BFF_BASE = import.meta.env.VITE_OAUTH_URL ?? 'http://localhost:5002'

interface DashboardCounts {
  businessCount: number
  businessUnitCount: number
  userCount: number
}

function IconBuilding({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.75" strokeLinecap="round" strokeLinejoin="round" className={className}>
      <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" />
      <polyline points="9 22 9 12 15 12 15 22" />
    </svg>
  )
}

function IconUsers({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.75" strokeLinecap="round" strokeLinejoin="round" className={className}>
      <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
      <circle cx="9" cy="7" r="4" />
      <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
      <path d="M16 3.13a4 4 0 0 1 0 7.75" />
    </svg>
  )
}

function IconShield({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.75" strokeLinecap="round" strokeLinejoin="round" className={className}>
      <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
    </svg>
  )
}

function IconArrowRight({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className}>
      <line x1="5" y1="12" x2="19" y2="12" />
      <polyline points="12 5 19 12 12 19" />
    </svg>
  )
}

function fmt(n: number | undefined): string {
  return n === undefined ? '—' : String(n)
}

function buildStats(counts: DashboardCounts | null) {
  return [
    {
      title: 'Empresas',
      value: fmt(counts?.businessCount),
      subtitle: 'Total cadastrado',
      icon: <IconShield className="w-6 h-6" />,
      bg: 'bg-warning-100',
      color: 'text-warning-600',
    },
    {
      title: 'Unidades de Negócio',
      value: fmt(counts?.businessUnitCount),
      subtitle: 'Total cadastrado',
      icon: <IconBuilding className="w-6 h-6" />,
      bg: 'bg-primary-100',
      color: 'text-primary-600',
    },
    {
      title: 'Usuários',
      value: fmt(counts?.userCount),
      subtitle: 'Contas registradas',
      icon: <IconUsers className="w-6 h-6" />,
      bg: 'bg-success-100',
      color: 'text-success-700',
    },
  ]
}

const modules = [
  {
    title: 'Empresas',
    description: 'Gerencie empresas e suas unidades de negócio (CNPJ, filiais).',
    path: '/business-units',
    icon: <IconBuilding className="w-7 h-7" />,
    bg: 'bg-primary-50',
    color: 'text-primary-600',
  },
  {
    title: 'Gestão de Usuários',
    description: 'Controle acessos, roles e status dos usuários da plataforma.',
    path: '/users',
    icon: <IconUsers className="w-7 h-7" />,
    bg: 'bg-violet-50',
    color: 'text-violet-600',
  },
]

export function Dashboard() {
  const { user, accessToken } = useAuth()
  const hour = new Date().getHours()
  const greeting = hour < 12 ? 'Bom dia' : hour < 18 ? 'Boa tarde' : 'Boa noite'

  const [counts, setCounts] = useState<DashboardCounts | null>(null)
  useEffect(() => {
    const token = accessToken ?? localStorage.getItem('access_token')
    if (!token) return
    fetch(`${BFF_BASE}/api/v1/dashboard`, { headers: { Authorization: `Bearer ${token}` } })
      .then((r) => (r.ok ? r.json() : null))
      .then((data) => data && setCounts(data))
      .catch(() => {})
  }, [accessToken])

  const stats = buildStats(counts)

  return (
    <div className="space-y-8">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-neutral-900">
          {greeting}{user?.given_name ? `, ${user.given_name}` : ''}!
        </h1>
        <p className="mt-1 text-sm text-neutral-500">
          Bem-vindo à plataforma. Aqui está um resumo do sistema.
        </p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
        {stats.map((s) => (
          <div key={s.title} className="flex items-center gap-4 rounded-xl bg-white p-5 shadow-card border border-neutral-200">
            <div className={`flex h-12 w-12 shrink-0 items-center justify-center rounded-xl ${s.bg} ${s.color}`}>
              {s.icon}
            </div>
            <div className="min-w-0">
              <p className="text-xs font-medium text-neutral-500 truncate">{s.title}</p>
              <p className="mt-0.5 text-xl font-bold text-neutral-900">{s.value}</p>
              <p className="text-xs text-neutral-400">{s.subtitle}</p>
            </div>
          </div>
        ))}
      </div>

      {/* Modules */}
      <div>
        <h2 className="mb-4 text-xs font-semibold uppercase tracking-widest text-neutral-400">
          Módulos disponíveis
        </h2>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {modules.map((mod) => (
            <Link
              key={mod.path}
              to={mod.path}
              className="group flex flex-col gap-5 rounded-xl bg-white p-6 shadow-card border border-neutral-200 transition-all hover:shadow-card-md hover:-translate-y-0.5"
            >
              <div className="flex items-start justify-between">
                <div className={`flex h-12 w-12 items-center justify-center rounded-xl ${mod.bg} ${mod.color}`}>
                  {mod.icon}
                </div>
                <span className={`flex h-7 w-7 items-center justify-center rounded-lg bg-neutral-100 text-neutral-400 opacity-0 group-hover:opacity-100 transition-all group-hover:bg-neutral-200`}>
                  <IconArrowRight className="w-3.5 h-3.5" />
                </span>
              </div>
              <div>
                <h3 className="font-semibold text-neutral-900">{mod.title}</h3>
                <p className="mt-1.5 text-sm text-neutral-500 leading-relaxed">{mod.description}</p>
              </div>
            </Link>
          ))}
        </div>
      </div>
    </div>
  )
}
