import { Link, useNavigate, useParams } from 'react-router-dom'
import { useBusinessUnit, useDeleteBusinessUnit } from '../hooks/useBusinessUnits'

const CONTACT_TYPE_LABELS: Record<string, { label: string; color: string }> = {
  Primary:    { label: 'Principal',  color: 'bg-primary-50 text-primary-700' },
  Secondary:  { label: 'Secundário', color: 'bg-neutral-100 text-neutral-700' },
  Technical:  { label: 'Técnico',    color: 'bg-blue-50 text-blue-700' },
  Commercial: { label: 'Comercial',  color: 'bg-amber-50 text-amber-700' },
}

export function BusinessUnitDetail() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data, isLoading, error } = useBusinessUnit(id!)
  const deleteMutation = useDeleteBusinessUnit()

  async function handleDelete() {
    if (!confirm(`Deseja excluir "${data?.razaoSocial}"?`)) return
    await deleteMutation.mutateAsync(id!)
    navigate('/business-units')
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-2 border-neutral-200 border-t-primary-600" />
      </div>
    )
  }

  if (error || !data) {
    return (
      <div className="rounded-xl bg-danger-50 border border-danger-100 p-4">
        <p className="text-sm font-medium text-danger-700">{error?.message ?? 'Não encontrado'}</p>
      </div>
    )
  }

  return (
    <div className="space-y-6 max-w-4xl">
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
            <div className="flex h-14 w-14 shrink-0 items-center justify-center rounded-2xl bg-primary-100 text-primary-600 text-xl font-bold select-none">
              {data.razaoSocial.charAt(0).toUpperCase()}
            </div>
            <div>
              <h1 className="text-2xl font-bold text-neutral-900">{data.razaoSocial}</h1>
              <p className="mt-0.5 text-sm text-neutral-500">{data.nomeFantasia}</p>
            </div>
          </div>
          <div className="flex items-center gap-2 shrink-0">
            <Link
              to={`/business-units/${id}/edit`}
              className="rounded-xl border border-neutral-200 px-4 py-2 text-sm font-semibold text-neutral-700 hover:bg-neutral-50 transition-colors"
            >
              Editar
            </Link>
            <button
              onClick={handleDelete}
              disabled={deleteMutation.isPending}
              className="rounded-xl bg-danger-50 px-4 py-2 text-sm font-semibold text-danger-700 hover:bg-danger-100 disabled:opacity-40 transition-colors"
            >
              Excluir
            </button>
          </div>
        </div>
      </div>

      {/* Cards */}
      <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
        <div className="rounded-xl bg-white border border-neutral-200 shadow-card p-5">
          <h2 className="text-xs font-semibold uppercase tracking-wider text-neutral-500 mb-4">Identificação</h2>
          <dl className="space-y-3">
            <div className="flex items-center justify-between">
              <dt className="text-sm text-neutral-500">CNPJ</dt>
              <dd className="font-mono text-sm font-semibold text-neutral-900 bg-neutral-100 px-2.5 py-1 rounded-lg">{data.cnpj}</dd>
            </div>
            <div className="flex items-center justify-between">
              <dt className="text-sm text-neutral-500">Cadastrado em</dt>
              <dd className="text-sm text-neutral-700">{new Date(data.createdAt).toLocaleString('pt-BR')}</dd>
            </div>
            {data.updatedAt && (
              <div className="flex items-center justify-between">
                <dt className="text-sm text-neutral-500">Atualizado em</dt>
                <dd className="text-sm text-neutral-700">{new Date(data.updatedAt).toLocaleString('pt-BR')}</dd>
              </div>
            )}
          </dl>
        </div>

        {data.address && (
          <div className="rounded-xl bg-white border border-neutral-200 shadow-card p-5">
            <h2 className="text-xs font-semibold uppercase tracking-wider text-neutral-500 mb-4">Endereço</h2>
            <address className="not-italic space-y-1 text-sm text-neutral-700">
              <p className="font-semibold text-neutral-900">
                {data.address.street}, {data.address.number}
                {data.address.complement && ` — ${data.address.complement}`}
              </p>
              <p>{data.address.district}</p>
              <p>{data.address.city} — {data.address.state}</p>
              <p className="font-mono text-xs text-neutral-500 mt-1">CEP {data.address.zipCode}</p>
            </address>
          </div>
        )}
      </div>

      {/* Contatos */}
      {data.contacts.length > 0 && (
        <div className="rounded-xl bg-white border border-neutral-200 shadow-card p-5">
          <h2 className="text-xs font-semibold uppercase tracking-wider text-neutral-500 mb-4">
            Contatos <span className="text-neutral-400 ml-1 normal-case font-normal">({data.contacts.length})</span>
          </h2>
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            {data.contacts.map((contact) => {
              const type = CONTACT_TYPE_LABELS[contact.type] ?? { label: contact.type, color: 'bg-neutral-100 text-neutral-700' }
              return (
                <div key={contact.id} className="flex flex-col gap-1.5 rounded-xl border border-neutral-100 bg-neutral-50 p-4">
                  <div className="flex items-center justify-between mb-0.5">
                    <p className="text-sm font-semibold text-neutral-900">{contact.name}</p>
                    <span className={`rounded-full px-2.5 py-0.5 text-xs font-semibold ${type.color}`}>{type.label}</span>
                  </div>
                  <p className="text-sm text-neutral-600">{contact.email}</p>
                  <p className="text-sm text-neutral-600">{contact.phone}</p>
                </div>
              )
            })}
          </div>
        </div>
      )}
    </div>
  )
}
