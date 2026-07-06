import { useForm } from 'react-hook-form'
import { useNavigate } from 'react-router-dom'
import { useCreateBusiness } from '../hooks/useBusinesses'
import type { CreateBusinessPayload } from '../types/business'

const inputCls = 'block w-full rounded-xl border border-neutral-200 bg-white px-3.5 py-2.5 text-sm text-neutral-900 placeholder-neutral-400 shadow-sm focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20'

export function BusinessCreate() {
  const navigate = useNavigate()
  const mutation = useCreateBusiness()
  const { register, handleSubmit, formState: { errors } } = useForm<CreateBusinessPayload>()

  async function onSubmit(data: CreateBusinessPayload) {
    const result = await mutation.mutateAsync(data)
    navigate(`/business-units/${result.id}`)
  }

  return (
    <div className="max-w-2xl space-y-6">
      <div className="space-y-1">
        <button onClick={() => navigate(-1)} className="text-sm text-neutral-500 hover:text-neutral-800">← Voltar</button>
        <h1 className="text-2xl font-bold text-neutral-900">Nova Empresa</h1>
        <p className="text-sm text-neutral-500">Cadastre a empresa. As unidades de negócio são criadas dentro dela.</p>
      </div>

      {mutation.error && (
        <div className="rounded-xl bg-danger-50 border border-danger-100 p-4">
          <p className="text-sm font-medium text-danger-700">{(mutation.error as Error).message}</p>
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="rounded-xl bg-white border border-neutral-200 shadow-card p-6 space-y-4">
        <div>
          <label className="block text-sm font-medium text-neutral-700 mb-1.5">Razão Social <span className="text-danger-500">*</span></label>
          <input {...register('razaoSocial', { required: 'Obrigatório' })} className={inputCls} placeholder="Acme Comércio LTDA" />
          {errors.razaoSocial && <p className="mt-1.5 text-xs text-danger-600">{errors.razaoSocial.message}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium text-neutral-700 mb-1.5">Nome Fantasia <span className="text-danger-500">*</span></label>
          <input {...register('nomeFantasia', { required: 'Obrigatório' })} className={inputCls} placeholder="Acme" />
          {errors.nomeFantasia && <p className="mt-1.5 text-xs text-danger-600">{errors.nomeFantasia.message}</p>}
        </div>
        <div>
          <label className="block text-sm font-medium text-neutral-700 mb-1.5">CNPJ <span className="text-danger-500">*</span></label>
          <input {...register('cnpj', { required: 'Obrigatório' })} className={inputCls} placeholder="11.222.333/0001-81" />
          {errors.cnpj && <p className="mt-1.5 text-xs text-danger-600">{errors.cnpj.message}</p>}
          <p className="mt-1.5 text-xs text-neutral-400">CNPJ válido (14 dígitos).</p>
        </div>
        <div className="flex justify-end border-t border-neutral-100 pt-4">
          <button type="submit" disabled={mutation.isPending} className="rounded-xl bg-primary-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-primary-700 disabled:opacity-50">
            {mutation.isPending ? 'Salvando...' : 'Criar Empresa'}
          </button>
        </div>
      </form>
    </div>
  )
}
