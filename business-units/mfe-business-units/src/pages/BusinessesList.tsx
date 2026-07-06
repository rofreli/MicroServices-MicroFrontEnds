import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useBusinesses, useDeleteBusiness } from '../hooks/useBusinesses'

export function BusinessesList() {
  const [page, setPage] = useState(1)
  const navigate = useNavigate()
  const { data, isLoading, error } = useBusinesses(page)
  const remove = useDeleteBusiness()

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-2 border-neutral-200 border-t-primary-600" />
      </div>
    )
  }

  if (error) {
    return (
      <div className="rounded-xl bg-danger-50 border border-danger-100 p-4">
        <p className="text-sm font-medium text-danger-700">{(error as Error).message}</p>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-bold text-neutral-900">Empresas</h1>
          <p className="mt-1 text-sm text-neutral-500">
            {data?.totalCount ?? 0} empresa{(data?.totalCount ?? 0) !== 1 ? 's' : ''} cadastrada{(data?.totalCount ?? 0) !== 1 ? 's' : ''}
          </p>
        </div>
        <Link
          to="/business-units/new"
          className="flex items-center gap-2 rounded-xl bg-primary-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm hover:bg-primary-700 transition-colors"
        >
          + Nova Empresa
        </Link>
      </div>

      {remove.error && (
        <div className="rounded-xl bg-danger-50 border border-danger-100 p-4">
          <p className="text-sm font-medium text-danger-700">{(remove.error as Error).message}</p>
        </div>
      )}

      {!data?.items.length ? (
        <div className="flex flex-col items-center justify-center rounded-xl bg-white border border-dashed border-neutral-300 py-20">
          <h3 className="text-sm font-semibold text-neutral-900">Nenhuma empresa cadastrada</h3>
          <p className="mt-1 text-sm text-neutral-500">Comece criando a primeira empresa.</p>
          <Link to="/business-units/new" className="mt-5 rounded-xl bg-primary-600 px-4 py-2 text-sm font-semibold text-white hover:bg-primary-700">
            Criar Empresa
          </Link>
        </div>
      ) : (
        <div className="rounded-xl bg-white border border-neutral-200 shadow-card overflow-hidden">
          <table className="min-w-full">
            <thead>
              <tr className="border-b border-neutral-200 bg-neutral-50">
                {['Razão Social', 'Nome Fantasia', 'CNPJ', 'Status', 'Ações'].map((h) => (
                  <th key={h} className={`px-6 py-3.5 text-xs font-semibold text-neutral-500 uppercase tracking-wider ${h === 'Ações' ? 'text-right' : 'text-left'}`}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-neutral-100">
              {data.items.map((b) => (
                <tr key={b.id} className="hover:bg-neutral-50 cursor-pointer transition-colors" onClick={() => navigate(`/business-units/${b.id}`)}>
                  <td className="px-6 py-4 text-sm font-semibold text-neutral-900">{b.razaoSocial}</td>
                  <td className="px-6 py-4 text-sm text-neutral-500">{b.nomeFantasia}</td>
                  <td className="px-6 py-4 text-sm text-neutral-500 font-mono">{b.cnpj}</td>
                  <td className="px-6 py-4">
                    <span className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-semibold ${b.isActive ? 'bg-success-50 text-success-700' : 'bg-danger-50 text-danger-700'}`}>
                      <span className={`h-1.5 w-1.5 rounded-full ${b.isActive ? 'bg-success-500' : 'bg-danger-500'}`} />
                      {b.isActive ? 'Ativa' : 'Inativa'}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-right" onClick={(e) => e.stopPropagation()}>
                    <div className="flex items-center justify-end gap-1">
                      <Link to={`/business-units/${b.id}/edit`} className="rounded-lg px-3 py-1.5 text-xs font-semibold text-primary-600 hover:bg-primary-50">Editar</Link>
                      <button
                        onClick={() => { if (confirm(`Excluir a empresa "${b.razaoSocial}"?`)) remove.mutate(b.id) }}
                        disabled={remove.isPending}
                        className="rounded-lg px-3 py-1.5 text-xs font-semibold text-danger-600 hover:bg-danger-50 disabled:opacity-40"
                      >
                        Excluir
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {(data.totalPages ?? 0) > 1 && (
            <div className="flex items-center justify-between border-t border-neutral-200 bg-white px-6 py-3.5">
              <p className="text-sm text-neutral-500">Página {data.page} de {data.totalPages}</p>
              <div className="flex gap-1.5">
                <button onClick={() => setPage((p) => p - 1)} disabled={!data.hasPreviousPage} className="rounded-lg border border-neutral-200 px-3 py-1.5 text-sm font-medium text-neutral-600 disabled:opacity-40 hover:bg-neutral-50">Anterior</button>
                <button onClick={() => setPage((p) => p + 1)} disabled={!data.hasNextPage} className="rounded-lg border border-neutral-200 px-3 py-1.5 text-sm font-medium text-neutral-600 disabled:opacity-40 hover:bg-neutral-50">Próxima</button>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  )
}
