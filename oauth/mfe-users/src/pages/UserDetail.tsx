import { useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { useUser, useDeactivateUser, useActivateUser, useAddRole, useRemoveRole } from '../hooks/useUsers'
import { AVAILABLE_ROLES } from '../types/user'

const AVATAR_COLORS = [
  'bg-violet-100 text-violet-600',
  'bg-blue-100 text-blue-600',
  'bg-emerald-100 text-emerald-600',
  'bg-amber-100 text-amber-600',
  'bg-rose-100 text-rose-600',
]

function avatarColor(name: string) {
  return AVATAR_COLORS[name.charCodeAt(0) % AVATAR_COLORS.length]
}

export function UserDetail() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data: user, isLoading, error } = useUser(id!)
  const deactivate = useDeactivateUser()
  const activate = useActivateUser()
  const addRole = useAddRole(id!)
  const removeRole = useRemoveRole(id!)
  const [selectedRole, setSelectedRole] = useState('')

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-2 border-neutral-200 border-t-primary-600" />
      </div>
    )
  }

  if (error || !user) {
    return (
      <div className="rounded-xl bg-danger-50 border border-danger-100 p-4">
        <p className="text-sm font-medium text-danger-700">{error?.message ?? 'Não encontrado'}</p>
      </div>
    )
  }

  const availableToAdd = AVAILABLE_ROLES.filter((r) => !user.roles.includes(r))

  return (
    <div className="space-y-6 max-w-3xl">
      {/* Header */}
      <div className="space-y-4">
        <button
          onClick={() => navigate(-1)}
          className="flex items-center gap-1.5 text-sm text-neutral-500 hover:text-neutral-800 transition-colors"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="w-4 h-4">
            <polyline points="15 18 9 12 15 6" />
          </svg>
          Voltar
        </button>
        <div className="flex items-start justify-between gap-4">
          <div className="flex items-start gap-4">
            <div className={`flex h-14 w-14 shrink-0 items-center justify-center rounded-2xl text-xl font-bold select-none ${avatarColor(user.fullName)}`}>
              {user.fullName.charAt(0).toUpperCase()}
            </div>
            <div>
              <h1 className="text-2xl font-bold text-neutral-900">{user.fullName}</h1>
              <p className="mt-0.5 text-sm text-neutral-500">{user.email}</p>
            </div>
          </div>
          <div className="flex items-center gap-2 shrink-0">
            <Link
              to={`/users/${id}/edit`}
              className="rounded-xl border border-neutral-200 px-4 py-2 text-sm font-semibold text-neutral-700 hover:bg-neutral-50 transition-colors"
            >
              Editar
            </Link>
            {user.isActive ? (
              <button
                onClick={() => deactivate.mutate(id!)}
                disabled={deactivate.isPending}
                className="rounded-xl bg-danger-50 px-4 py-2 text-sm font-semibold text-danger-700 hover:bg-danger-100 disabled:opacity-40 transition-colors"
              >
                Inativar
              </button>
            ) : (
              <button
                onClick={() => activate.mutate(id!)}
                disabled={activate.isPending}
                className="rounded-xl bg-success-50 px-4 py-2 text-sm font-semibold text-success-700 hover:bg-success-100 disabled:opacity-40 transition-colors"
              >
                Ativar
              </button>
            )}
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
        {/* Informações */}
        <div className="rounded-xl bg-white border border-neutral-200 shadow-card p-5">
          <h2 className="text-xs font-semibold uppercase tracking-wider text-neutral-500 mb-4">Informações</h2>
          <dl className="space-y-3">
            <div className="flex items-center justify-between">
              <dt className="text-sm text-neutral-500">Status</dt>
              <dd>
                <span className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-semibold ${user.isActive ? 'bg-success-50 text-success-700' : 'bg-danger-50 text-danger-700'}`}>
                  <span className={`h-1.5 w-1.5 rounded-full ${user.isActive ? 'bg-success-500' : 'bg-danger-500'}`} />
                  {user.isActive ? 'Ativo' : 'Inativo'}
                </span>
              </dd>
            </div>
            <div className="flex items-center justify-between">
              <dt className="text-sm text-neutral-500">Cadastrado em</dt>
              <dd className="text-sm text-neutral-700">{new Date(user.createdAt).toLocaleString('pt-BR')}</dd>
            </div>
            {user.updatedAt && (
              <div className="flex items-center justify-between">
                <dt className="text-sm text-neutral-500">Atualizado em</dt>
                <dd className="text-sm text-neutral-700">{new Date(user.updatedAt).toLocaleString('pt-BR')}</dd>
              </div>
            )}
            {user.externalProviders.length > 0 && (
              <div className="flex items-center justify-between">
                <dt className="text-sm text-neutral-500">SSO</dt>
                <dd className="flex gap-1.5">
                  {user.externalProviders.map((p) => (
                    <span key={p} className="rounded-lg bg-neutral-100 px-2 py-0.5 text-xs font-semibold text-neutral-700">{p}</span>
                  ))}
                </dd>
              </div>
            )}
          </dl>
        </div>

        {/* Roles */}
        <div className="rounded-xl bg-white border border-neutral-200 shadow-card p-5">
          <h2 className="text-xs font-semibold uppercase tracking-wider text-neutral-500 mb-4">Permissões</h2>

          <div className="flex flex-wrap gap-2 min-h-[2.5rem] mb-3">
            {user.roles.length === 0 ? (
              <p className="text-sm text-neutral-400">Nenhuma role atribuída.</p>
            ) : (
              user.roles.map((role) => (
                <span key={role} className="inline-flex items-center gap-1.5 rounded-full bg-primary-50 pl-3 pr-2 py-1 text-xs font-semibold text-primary-700">
                  {role}
                  <button
                    onClick={() => removeRole.mutate(role)}
                    disabled={removeRole.isPending}
                    className="flex h-4 w-4 items-center justify-center rounded-full text-primary-400 hover:bg-primary-200 hover:text-primary-800 transition-colors disabled:opacity-40"
                    title={`Remover ${role}`}
                  >
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" className="w-2.5 h-2.5">
                      <line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" />
                    </svg>
                  </button>
                </span>
              ))
            )}
          </div>

          {availableToAdd.length > 0 && (
            <div className="flex gap-2 border-t border-neutral-100 pt-3">
              <select
                value={selectedRole}
                onChange={(e) => setSelectedRole(e.target.value)}
                className="flex-1 rounded-xl border border-neutral-200 bg-white px-3 py-1.5 text-sm text-neutral-700 focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20"
              >
                <option value="">Adicionar role...</option>
                {availableToAdd.map((r) => (
                  <option key={r} value={r}>{r}</option>
                ))}
              </select>
              <button
                onClick={() => {
                  if (selectedRole) { addRole.mutate(selectedRole); setSelectedRole('') }
                }}
                disabled={!selectedRole || addRole.isPending}
                className="rounded-xl bg-primary-600 px-3 py-1.5 text-sm font-semibold text-white hover:bg-primary-700 disabled:opacity-40 transition-colors"
              >
                Adicionar
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
