import { useForm } from 'react-hook-form'
import { AVAILABLE_ROLES, type CreateUserPayload } from '../types/user'

interface Props {
  defaultValues?: Partial<CreateUserPayload>
  onSubmit: (data: CreateUserPayload) => void
  isLoading?: boolean
  submitLabel?: string
  showEmail?: boolean
  showPassword?: boolean
  showRoles?: boolean
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

export function UserForm({
  defaultValues,
  onSubmit,
  isLoading,
  submitLabel = 'Salvar',
  showEmail = true,
  showPassword = true,
  showRoles = true,
}: Props) {
  const { register, handleSubmit, formState: { errors } } =
    useForm<CreateUserPayload>({ defaultValues })

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-8">
      {/* Dados pessoais */}
      <section className="space-y-4">
        <div className="border-b border-neutral-100 pb-3">
          <h3 className="text-sm font-semibold text-neutral-900">Dados Pessoais</h3>
          <p className="mt-0.5 text-xs text-neutral-500">Informações de identificação do usuário</p>
        </div>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div>
            <Label required>Nome</Label>
            <input
              {...register('firstName', { required: 'Obrigatório' })}
              className={inputCls}
              placeholder="João"
            />
            {errors.firstName && <p className={errorCls}>{errors.firstName.message}</p>}
          </div>
          <div>
            <Label required>Sobrenome</Label>
            <input
              {...register('lastName', { required: 'Obrigatório' })}
              className={inputCls}
              placeholder="Silva"
            />
            {errors.lastName && <p className={errorCls}>{errors.lastName.message}</p>}
          </div>

          {showEmail && (
            <div className="sm:col-span-2">
              <Label required>E-mail</Label>
              <input
                {...register('email', {
                  required: 'Obrigatório',
                  pattern: { value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, message: 'E-mail inválido' },
                })}
                type="email"
                className={inputCls}
                placeholder="joao@empresa.com"
              />
              {errors.email && <p className={errorCls}>{errors.email.message}</p>}
            </div>
          )}

          {showPassword && (
            <div className="sm:col-span-2">
              <Label required>Senha</Label>
              <input
                {...register('password', {
                  required: 'Obrigatório',
                  minLength: { value: 8, message: 'Mínimo 8 caracteres' },
                })}
                type="password"
                className={inputCls}
                placeholder="••••••••"
              />
              {errors.password && <p className={errorCls}>{errors.password.message}</p>}
              <p className="mt-1.5 text-xs text-neutral-400">Mínimo 8 caracteres.</p>
            </div>
          )}
        </div>
      </section>

      {/* Roles */}
      {showRoles && (
        <section className="space-y-4">
          <div className="border-b border-neutral-100 pb-3">
            <h3 className="text-sm font-semibold text-neutral-900">Permissões</h3>
            <p className="mt-0.5 text-xs text-neutral-500">Roles de acesso atribuídas ao usuário</p>
          </div>
          <div className="flex flex-wrap gap-3">
            {AVAILABLE_ROLES.map((role) => (
              <label key={role} className="flex items-center gap-2.5 cursor-pointer rounded-xl border border-neutral-200 bg-neutral-50 px-4 py-2.5 hover:bg-neutral-100 transition-colors has-[:checked]:border-primary-300 has-[:checked]:bg-primary-50">
                <input
                  type="checkbox"
                  value={role}
                  {...register('roles')}
                  className="rounded border-neutral-300 text-primary-600 focus:ring-primary-500 focus:ring-offset-0"
                />
                <span className="text-sm font-medium text-neutral-700">{role}</span>
              </label>
            ))}
          </div>
        </section>
      )}

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
