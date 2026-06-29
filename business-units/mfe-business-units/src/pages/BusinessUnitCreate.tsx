import { useNavigate } from 'react-router-dom'
import { BusinessUnitForm } from '../components/BusinessUnitForm'
import { useCreateBusinessUnit } from '../hooks/useBusinessUnits'
import type { CreateBusinessUnitPayload } from '../types/businessUnit'

export function BusinessUnitCreate() {
  const navigate = useNavigate()
  const mutation = useCreateBusinessUnit()

  async function handleSubmit(data: CreateBusinessUnitPayload) {
    const result = await mutation.mutateAsync(data)
    navigate(`/business-units/${result.id}`)
  }

  return (
    <div className="max-w-3xl space-y-6">
      <div className="space-y-1">
        <button
          onClick={() => navigate(-1)}
          className="flex items-center gap-1.5 text-sm text-neutral-500 hover:text-neutral-800 transition-colors"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="w-4 h-4">
            <polyline points="15 18 9 12 15 6" />
          </svg>
          Voltar
        </button>
        <h1 className="text-2xl font-bold text-neutral-900">Nova Unidade de Negócio</h1>
        <p className="text-sm text-neutral-500">Preencha os dados abaixo para cadastrar uma nova unidade.</p>
      </div>

      {mutation.error && (
        <div className="rounded-xl bg-danger-50 border border-danger-100 p-4">
          <p className="text-sm font-medium text-danger-700">{mutation.error.message}</p>
        </div>
      )}

      <div className="rounded-xl bg-white border border-neutral-200 shadow-card p-6">
        <BusinessUnitForm
          onSubmit={handleSubmit}
          isLoading={mutation.isPending}
          submitLabel="Criar Unidade"
        />
      </div>
    </div>
  )
}
