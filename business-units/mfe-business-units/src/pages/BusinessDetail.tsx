import { useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
  useBusiness,
  useBusinessUnits,
  useCreateUnit,
  useDeleteUnit,
} from '../hooks/useBusinesses'

const inputCls = 'rounded-xl border border-neutral-200 bg-white px-3 py-2 text-sm text-neutral-900 placeholder-neutral-400 focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20'

export function BusinessDetail() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data: business, isLoading, error } = useBusiness(id!)
  const { data: units } = useBusinessUnits(id!)
  const createUnit = useCreateUnit(id!)
  const deleteUnit = useDeleteUnit(id!)

  const [razaoSocial, setRazaoSocial] = useState('')
  const [nomeFantasia, setNomeFantasia] = useState('')
  const [cnpj, setCnpj] = useState('')

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-2 border-neutral-200 border-t-primary-600" />
      </div>
    )
  }
  if (error || !business) {
    return (
      <div className="rounded-xl bg-danger-50 border border-danger-100 p-4">
        <p className="text-sm font-medium text-danger-700">{(error as Error)?.message ?? 'Não encontrada'}</p>
      </div>
    )
  }

  const canCreate = razaoSocial.trim() && nomeFantasia.trim() && cnpj.trim()

  return (
    <div className="space-y-6 max-w-3xl">
      <div className="space-y-4">
        <button onClick={() => navigate('/business-units')} className="text-sm text-neutral-500 hover:text-neutral-800">← Empresas</button>
        <div className="flex items-start justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold text-neutral-900">{business.razaoSocial}</h1>
            <p className="mt-0.5 text-sm text-neutral-500">{business.nomeFantasia} · <span className="font-mono">{business.cnpj}</span></p>
          </div>
          <Link to={`/business-units/${id}/edit`} className="rounded-xl border border-neutral-200 px-4 py-2 text-sm font-semibold text-neutral-700 hover:bg-neutral-50">Editar</Link>
        </div>
      </div>

      {/* Business Units */}
      <div className="rounded-xl bg-white border border-neutral-200 shadow-card p-5">
        <h2 className="text-xs font-semibold uppercase tracking-wider text-neutral-500 mb-4">
          Unidades de Negócio {units && <span className="text-neutral-400">({units.totalCount})</span>}
        </h2>

        {(createUnit.error || deleteUnit.error) && (
          <div className="mb-3 rounded-lg bg-danger-50 border border-danger-100 px-3 py-2">
            <p className="text-xs font-medium text-danger-700">{((createUnit.error ?? deleteUnit.error) as Error)?.message}</p>
          </div>
        )}

        <div className="space-y-2 mb-4">
          {!units?.items.length ? (
            <p className="text-sm text-neutral-400">Nenhuma unidade cadastrada nesta empresa.</p>
          ) : (
            units.items.map((u) => (
              <div key={u.id} className="flex items-center justify-between rounded-xl border border-neutral-100 bg-neutral-50 px-3 py-2">
                <div className="text-sm">
                  <span className="font-semibold text-neutral-900">{u.razaoSocial}</span>
                  <span className="text-neutral-500"> · {u.nomeFantasia} · <span className="font-mono">{u.cnpj}</span></span>
                </div>
                <button
                  onClick={() => { if (confirm(`Excluir a unidade "${u.razaoSocial}"?`)) deleteUnit.mutate(u.id) }}
                  disabled={deleteUnit.isPending}
                  className="rounded-lg px-2 py-1 text-xs font-semibold text-danger-600 hover:bg-danger-50 disabled:opacity-40"
                >
                  Excluir
                </button>
              </div>
            ))
          )}
        </div>

        {/* Add unit */}
        <div className="flex flex-wrap items-end gap-2 border-t border-neutral-100 pt-4">
          <div className="flex-1 min-w-[140px]">
            <label className="block text-xs font-medium text-neutral-600 mb-1">Razão Social</label>
            <input value={razaoSocial} onChange={(e) => setRazaoSocial(e.target.value)} className={`${inputCls} w-full`} placeholder="Filial SP LTDA" />
          </div>
          <div className="flex-1 min-w-[120px]">
            <label className="block text-xs font-medium text-neutral-600 mb-1">Nome Fantasia</label>
            <input value={nomeFantasia} onChange={(e) => setNomeFantasia(e.target.value)} className={`${inputCls} w-full`} placeholder="Filial SP" />
          </div>
          <div className="flex-1 min-w-[140px]">
            <label className="block text-xs font-medium text-neutral-600 mb-1">CNPJ</label>
            <input value={cnpj} onChange={(e) => setCnpj(e.target.value)} className={`${inputCls} w-full`} placeholder="11.222.333/0001-81" />
          </div>
          <button
            onClick={() => {
              if (!canCreate) return
              createUnit.mutate(
                { razaoSocial: razaoSocial.trim(), nomeFantasia: nomeFantasia.trim(), cnpj: cnpj.trim() },
                { onSuccess: () => { setRazaoSocial(''); setNomeFantasia(''); setCnpj('') } },
              )
            }}
            disabled={!canCreate || createUnit.isPending}
            className="rounded-xl bg-primary-600 px-4 py-2 text-sm font-semibold text-white hover:bg-primary-700 disabled:opacity-40"
          >
            Adicionar Unidade
          </button>
        </div>
      </div>
    </div>
  )
}
