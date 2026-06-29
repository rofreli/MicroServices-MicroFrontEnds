import { useFieldArray, useForm } from 'react-hook-form'
import type { CreateBusinessUnitPayload } from '../types/businessUnit'

interface Props {
  defaultValues?: Partial<CreateBusinessUnitPayload>
  onSubmit: (data: CreateBusinessUnitPayload) => void
  isLoading?: boolean
  submitLabel?: string
  showCnpj?: boolean
}

function Label({ children, required }: { children: React.ReactNode; required?: boolean }) {
  return (
    <label className="block text-sm font-medium text-neutral-700 mb-1.5">
      {children}
      {required && <span className="ml-0.5 text-danger-500">*</span>}
    </label>
  )
}

const inputCls = 'block w-full rounded-xl border border-neutral-200 bg-white px-3.5 py-2.5 text-sm text-neutral-900 placeholder-neutral-400 shadow-sm transition-colors focus:border-primary-500 focus:outline-none focus:ring-2 focus:ring-primary-500/20'
const errorCls = 'mt-1.5 text-xs text-danger-600'

export function BusinessUnitForm({
  defaultValues,
  onSubmit,
  isLoading,
  submitLabel = 'Salvar',
  showCnpj = true,
}: Props) {
  const { register, control, handleSubmit, formState: { errors } } =
    useForm<CreateBusinessUnitPayload>({ defaultValues })

  const { fields, append, remove } = useFieldArray({ control, name: 'contacts' })

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-8">
      {/* Identificação */}
      <section className="space-y-4">
        <div className="border-b border-neutral-100 pb-3">
          <h3 className="text-sm font-semibold text-neutral-900">Identificação</h3>
          <p className="mt-0.5 text-xs text-neutral-500">Dados principais da empresa</p>
        </div>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div>
            <Label required>Razão Social</Label>
            <input
              {...register('razaoSocial', { required: 'Obrigatório' })}
              className={inputCls}
              placeholder="Razão Social Ltda"
            />
            {errors.razaoSocial && <p className={errorCls}>{errors.razaoSocial.message}</p>}
          </div>
          <div>
            <Label required>Nome Fantasia</Label>
            <input
              {...register('nomeFantasia', { required: 'Obrigatório' })}
              className={inputCls}
              placeholder="Nome da empresa"
            />
            {errors.nomeFantasia && <p className={errorCls}>{errors.nomeFantasia.message}</p>}
          </div>
          {showCnpj && (
            <div>
              <Label required>CNPJ</Label>
              <input
                {...register('cnpj', {
                  required: 'Obrigatório',
                  pattern: { value: /^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}-?\d{2}$/, message: 'CNPJ inválido' },
                })}
                className={`${inputCls} font-mono`}
                placeholder="00.000.000/0000-00"
              />
              {errors.cnpj && <p className={errorCls}>{errors.cnpj.message}</p>}
            </div>
          )}
        </div>
      </section>

      {/* Endereço */}
      <section className="space-y-4">
        <div className="border-b border-neutral-100 pb-3">
          <h3 className="text-sm font-semibold text-neutral-900">Endereço</h3>
          <p className="mt-0.5 text-xs text-neutral-500">Localização física da unidade</p>
        </div>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <div className="sm:col-span-2">
            <Label>Logradouro</Label>
            <input {...register('address.street')} className={inputCls} placeholder="Rua, Avenida..." />
          </div>
          <div>
            <Label>Número</Label>
            <input {...register('address.number')} className={inputCls} placeholder="100" />
          </div>
          <div>
            <Label>Complemento</Label>
            <input {...register('address.complement')} className={inputCls} placeholder="Apto, Sala..." />
          </div>
          <div>
            <Label>Bairro</Label>
            <input {...register('address.district')} className={inputCls} placeholder="Bairro" />
          </div>
          <div>
            <Label>Cidade</Label>
            <input {...register('address.city')} className={inputCls} placeholder="São Paulo" />
          </div>
          <div>
            <Label>Estado</Label>
            <input {...register('address.state')} maxLength={2} className={`${inputCls} uppercase`} placeholder="SP" />
          </div>
          <div>
            <Label>CEP</Label>
            <input {...register('address.zipCode')} className={`${inputCls} font-mono`} placeholder="00000-000" />
          </div>
        </div>
      </section>

      {/* Contatos */}
      <section className="space-y-4">
        <div className="flex items-center justify-between border-b border-neutral-100 pb-3">
          <div>
            <h3 className="text-sm font-semibold text-neutral-900">Contatos</h3>
            <p className="mt-0.5 text-xs text-neutral-500">{fields.length} contato{fields.length !== 1 ? 's' : ''}</p>
          </div>
          <button
            type="button"
            onClick={() => append({ name: '', email: '', phone: '', type: 'Primary' })}
            className="flex items-center gap-1.5 rounded-lg border border-neutral-200 px-3 py-1.5 text-xs font-semibold text-neutral-700 hover:bg-neutral-50 transition-colors"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" className="w-3.5 h-3.5">
              <line x1="12" y1="5" x2="12" y2="19" /><line x1="5" y1="12" x2="19" y2="12" />
            </svg>
            Adicionar
          </button>
        </div>

        {fields.length === 0 && (
          <p className="text-sm text-neutral-400 text-center py-6">Nenhum contato adicionado.</p>
        )}

        <div className="space-y-3">
          {fields.map((field, index) => (
            <div key={field.id} className="rounded-xl border border-neutral-200 bg-neutral-50 p-4 space-y-3">
              <div className="flex items-center justify-between">
                <span className="text-xs font-semibold uppercase tracking-wide text-neutral-500">Contato {index + 1}</span>
                <button
                  type="button"
                  onClick={() => remove(index)}
                  className="text-xs font-semibold text-danger-600 hover:text-danger-700 transition-colors"
                >
                  Remover
                </button>
              </div>
              <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
                <div>
                  <Label>Nome</Label>
                  <input {...register(`contacts.${index}.name`)} className={inputCls} placeholder="Nome completo" />
                </div>
                <div>
                  <Label>E-mail</Label>
                  <input {...register(`contacts.${index}.email`)} type="email" className={inputCls} placeholder="email@empresa.com" />
                </div>
                <div>
                  <Label>Telefone</Label>
                  <input {...register(`contacts.${index}.phone`)} className={inputCls} placeholder="(11) 99999-9999" />
                </div>
                <div>
                  <Label>Tipo</Label>
                  <select {...register(`contacts.${index}.type`)} className={inputCls}>
                    <option value="Primary">Principal</option>
                    <option value="Secondary">Secundário</option>
                    <option value="Technical">Técnico</option>
                    <option value="Commercial">Comercial</option>
                  </select>
                </div>
              </div>
            </div>
          ))}
        </div>
      </section>

      {/* Actions */}
      <div className="flex justify-end gap-3 pt-2 border-t border-neutral-200">
        <button
          type="submit"
          disabled={isLoading}
          className="flex items-center gap-2 rounded-xl bg-primary-600 px-6 py-2.5 text-sm font-semibold text-white shadow-sm hover:bg-primary-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          {isLoading && (
            <svg className="animate-spin w-4 h-4" viewBox="0 0 24 24" fill="none">
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
              <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
            </svg>
          )}
          {isLoading ? 'Salvando...' : submitLabel}
        </button>
      </div>
    </form>
  )
}
