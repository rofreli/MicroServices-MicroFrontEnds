import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useBusinessUnits, useDeleteBusinessUnit } from '../hooks/useBusinessUnits'

function IconPlus() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="w-4 h-4">
      <line x1="12" y1="5" x2="12" y2="19" /><line x1="5" y1="12" x2="19" y2="12" />
    </svg>
  )
}

function IconChevronLeft() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="w-4 h-4">
      <polyline points="15 18 9 12 15 6" />
    </svg>
  )
}

function IconChevronRight() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="w-4 h-4">
      <polyline points="9 18 15 12 9 6" />
    </svg>
  )
}

const AVATAR_COLORS = [
  'bg-primary-100 text-primary-600',
  'bg-violet-100 text-violet-600',
  'bg-emerald-100 text-emerald-600',
  'bg-amber-100 text-amber-600',
  'bg-rose-100 text-rose-600',
]

function avatarColor(name: string) {
  return AVATAR_COLORS[name.charCodeAt(0) % AVATAR_COLORS.length]
}

export function BusinessUnitsList() {
  const [page, setPage] = useState(1)
  const navigate = useNavigate()
  const { data, isLoading, error } = useBusinessUnits(page)
  const deleteMutation = useDeleteBusinessUnit()

  async function handleDelete(id: string, name: string) {
    if (!confirm(`Deseja excluir "${name}"?`)) return
    await deleteMutation.mutateAsync(id)
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="flex flex-col items-center gap-3">
          <div className="animate-spin rounded-full h-8 w-8 border-2 border-neutral-200 border-t-primary-600" />
          <p className="text-sm text-neutral-400">Carregando...</p>
        </div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="rounded-xl bg-danger-50 border border-danger-100 p-4">
        <p className="text-sm font-medium text-danger-700">{error.message}</p>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Page header */}
      <div className="flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-bold text-neutral-900">Unidades de Negócio</h1>
          <p className="mt-1 text-sm text-neutral-500">
            {data?.totalCount ?? 0} unidade{(data?.totalCount ?? 0) !== 1 ? 's' : ''} cadastrada{(data?.totalCount ?? 0) !== 1 ? 's' : ''}
          </p>
        </div>
        <Link
          to="/business-units/new"
          className="flex items-center gap-2 rounded-xl bg-primary-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm hover:bg-primary-700 transition-colors"
        >
          <IconPlus />
          Nova Unidade
        </Link>
      </div>

      {/* Empty state */}
      {!data?.items.length ? (
        <div className="flex flex-col items-center justify-center rounded-xl bg-white border border-dashed border-neutral-300 py-20">
          <div className="flex h-14 w-14 items-center justify-center rounded-2xl bg-neutral-100">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" className="w-7 h-7 text-neutral-400">
              <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" />
              <polyline points="9 22 9 12 15 12 15 22" />
            </svg>
          </div>
          <h3 className="mt-4 text-sm font-semibold text-neutral-900">Nenhuma unidade cadastrada</h3>
          <p className="mt-1 text-sm text-neutral-500">Comece criando a primeira unidade de negócio.</p>
          <Link
            to="/business-units/new"
            className="mt-5 flex items-center gap-2 rounded-xl bg-primary-600 px-4 py-2 text-sm font-semibold text-white hover:bg-primary-700 transition-colors"
          >
            <IconPlus />
            Criar Unidade
          </Link>
        </div>
      ) : (
        <div className="rounded-xl bg-white border border-neutral-200 shadow-card overflow-hidden">
          <table className="min-w-full">
            <thead>
              <tr className="border-b border-neutral-200 bg-neutral-50">
                <th className="px-6 py-3.5 text-left text-xs font-semibold text-neutral-500 uppercase tracking-wider">Empresa</th>
                <th className="px-6 py-3.5 text-left text-xs font-semibold text-neutral-500 uppercase tracking-wider">CNPJ</th>
                <th className="px-6 py-3.5 text-left text-xs font-semibold text-neutral-500 uppercase tracking-wider">Cadastro</th>
                <th className="px-6 py-3.5 text-right text-xs font-semibold text-neutral-500 uppercase tracking-wider">Ações</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-neutral-100">
              {data.items.map((bu) => (
                <tr
                  key={bu.id}
                  className="hover:bg-neutral-50 cursor-pointer transition-colors"
                  onClick={() => navigate(`/business-units/${bu.id}`)}
                >
                  <td className="px-6 py-4">
                    <div className="flex items-center gap-3">
                      <div className={`flex h-9 w-9 shrink-0 items-center justify-center rounded-xl text-sm font-bold select-none ${avatarColor(bu.razaoSocial)}`}>
                        {bu.razaoSocial.charAt(0).toUpperCase()}
                      </div>
                      <div>
                        <p className="text-sm font-semibold text-neutral-900">{bu.razaoSocial}</p>
                        <p className="text-xs text-neutral-400">{bu.nomeFantasia}</p>
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <span className="inline-flex items-center rounded-lg bg-neutral-100 px-2.5 py-1 text-xs font-mono font-medium text-neutral-700">
                      {bu.cnpj}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm text-neutral-500">
                    {new Date(bu.createdAt).toLocaleDateString('pt-BR')}
                  </td>
                  <td className="px-6 py-4 text-right" onClick={(e) => e.stopPropagation()}>
                    <div className="flex items-center justify-end gap-1">
                      <Link
                        to={`/business-units/${bu.id}/edit`}
                        className="inline-flex items-center rounded-lg px-3 py-1.5 text-xs font-semibold text-primary-600 hover:bg-primary-50 transition-colors"
                      >
                        Editar
                      </Link>
                      <button
                        onClick={() => handleDelete(bu.id, bu.razaoSocial)}
                        disabled={deleteMutation.isPending}
                        className="inline-flex items-center rounded-lg px-3 py-1.5 text-xs font-semibold text-danger-600 hover:bg-danger-50 transition-colors disabled:opacity-40"
                      >
                        Excluir
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {data.totalPages > 1 && (
            <div className="flex items-center justify-between border-t border-neutral-200 bg-white px-6 py-3.5">
              <p className="text-sm text-neutral-500">
                Página <span className="font-semibold text-neutral-900">{data.page}</span> de{' '}
                <span className="font-semibold text-neutral-900">{data.totalPages}</span>
              </p>
              <div className="flex gap-1.5">
                <button
                  onClick={() => setPage((p) => p - 1)}
                  disabled={!data.hasPreviousPage}
                  className="flex items-center gap-1 rounded-lg border border-neutral-200 px-3 py-1.5 text-sm font-medium text-neutral-600 disabled:opacity-40 hover:bg-neutral-50 transition-colors"
                >
                  <IconChevronLeft />
                  Anterior
                </button>
                <button
                  onClick={() => setPage((p) => p + 1)}
                  disabled={!data.hasNextPage}
                  className="flex items-center gap-1 rounded-lg border border-neutral-200 px-3 py-1.5 text-sm font-medium text-neutral-600 disabled:opacity-40 hover:bg-neutral-50 transition-colors"
                >
                  Próxima
                  <IconChevronRight />
                </button>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  )
}
