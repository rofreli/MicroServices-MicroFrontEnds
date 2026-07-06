import { useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
  useUser,
  useDeactivateUser,
  useActivateUser,
  useAddPermission,
  useRemovePermission,
} from '../hooks/useUsers'
import { MODULES, ROLES } from '../types/user'

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

const selectCls =
  'rounded-xl border border-neutral-200 bg-white px-3 py-1.5 text-sm text-neutral-700 focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20'

export function UserDetail() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data: user, isLoading, error } = useUser(id!)
  const deactivate = useDeactivateUser()
  const activate = useActivateUser()
  const addPermission = useAddPermission(id!)
  const removePermission = useRemovePermission(id!)

  const [businessId, setBusinessId] = useState('')
  const [module, setModule] = useState<string>(MODULES[0])
  const [role, setRole] = useState<string>(ROLES[0])

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

  const canAdd = businessId.trim().length > 0

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

      {/* Informações */}
      <div className="rounded-xl bg-white border border-neutral-200 shadow-card p-5">
        <h2 className="text-xs font-semibold uppercase tracking-wider text-neutral-500 mb-4">Informações</h2>
        <dl className="grid grid-cols-1 gap-3 sm:grid-cols-2">
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
            <dt className="text-sm text-neutral-500">Tipo</dt>
            <dd>
              <span className={`rounded-full px-2.5 py-0.5 text-xs font-semibold ${user.isSuperAdmin ? 'bg-violet-50 text-violet-700' : 'bg-neutral-100 text-neutral-600'}`}>
                {user.isSuperAdmin ? 'Super Admin' : 'Padrão'}
              </span>
            </dd>
          </div>
          <div className="flex items-center justify-between">
            <dt className="text-sm text-neutral-500">Cadastrado em</dt>
            <dd className="text-sm text-neutral-700">{new Date(user.createdAt).toLocaleString('pt-BR')}</dd>
          </div>
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

      {/* Permissões */}
      <div className="rounded-xl bg-white border border-neutral-200 shadow-card p-5">
        <h2 className="text-xs font-semibold uppercase tracking-wider text-neutral-500 mb-4">
          Permissões {user.isSuperAdmin && <span className="ml-1 text-violet-600">(Super Admin — acesso total)</span>}
        </h2>

        {(addPermission.error || removePermission.error) && (
          <div className="mb-3 rounded-lg bg-danger-50 border border-danger-100 px-3 py-2">
            <p className="text-xs font-medium text-danger-700">
              {(addPermission.error ?? removePermission.error)?.message}
            </p>
          </div>
        )}

        <div className="space-y-2 min-h-[2.5rem] mb-4">
          {user.permissions.length === 0 ? (
            <p className="text-sm text-neutral-400">Nenhuma permissão atribuída.</p>
          ) : (
            user.permissions.map((p, i) => (
              <div
                key={`${p.businessId}-${p.businessUnitId ?? ''}-${p.module}-${p.function ?? ''}-${i}`}
                className="flex items-center justify-between rounded-xl border border-neutral-100 bg-neutral-50 px-3 py-2"
              >
                <div className="flex flex-wrap items-center gap-2 text-sm">
                  <span className="rounded-full bg-primary-50 px-2.5 py-0.5 text-xs font-semibold text-primary-700">{p.module}</span>
                  <span className="rounded-full bg-blue-50 px-2.5 py-0.5 text-xs font-semibold text-blue-700">{p.role}</span>
                  {p.function && (
                    <span className="rounded-full bg-amber-50 px-2.5 py-0.5 text-xs font-semibold text-amber-700">{p.function}</span>
                  )}
                  <span className="text-xs text-neutral-500">
                    Empresa <span className="font-mono">{p.businessId}</span>
                    {p.businessUnitId && <> · BU <span className="font-mono">{p.businessUnitId}</span></>}
                  </span>
                </div>
                <button
                  onClick={() =>
                    removePermission.mutate({
                      businessId: p.businessId,
                      businessUnitId: p.businessUnitId ?? null,
                      module: p.module,
                      function: p.function ?? null,
                    })
                  }
                  disabled={removePermission.isPending}
                  className="ml-2 rounded-lg px-2 py-1 text-xs font-semibold text-danger-600 hover:bg-danger-50 disabled:opacity-40 transition-colors"
                >
                  Remover
                </button>
              </div>
            ))
          )}
        </div>

        {/* Add permission */}
        <div className="flex flex-wrap items-end gap-2 border-t border-neutral-100 pt-4">
          <div className="flex-1 min-w-[180px]">
            <label className="block text-xs font-medium text-neutral-600 mb-1">ID da Empresa</label>
            <input
              value={businessId}
              onChange={(e) => setBusinessId(e.target.value)}
              placeholder="business id"
              className={`${selectCls} w-full`}
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-neutral-600 mb-1">Módulo</label>
            <select value={module} onChange={(e) => setModule(e.target.value)} className={selectCls}>
              {MODULES.map((m) => <option key={m} value={m}>{m}</option>)}
            </select>
          </div>
          <div>
            <label className="block text-xs font-medium text-neutral-600 mb-1">Papel</label>
            <select value={role} onChange={(e) => setRole(e.target.value)} className={selectCls}>
              {ROLES.map((r) => <option key={r} value={r}>{r}</option>)}
            </select>
          </div>
          <button
            onClick={() => {
              if (!canAdd) return
              addPermission.mutate(
                { businessId: businessId.trim(), module, role },
                { onSuccess: () => setBusinessId('') },
              )
            }}
            disabled={!canAdd || addPermission.isPending}
            className="rounded-xl bg-primary-600 px-4 py-2 text-sm font-semibold text-white hover:bg-primary-700 disabled:opacity-40 transition-colors"
          >
            Adicionar
          </button>
        </div>
      </div>
    </div>
  )
}
