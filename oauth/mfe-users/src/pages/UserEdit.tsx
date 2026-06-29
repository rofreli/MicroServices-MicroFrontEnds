import { useNavigate, useParams } from 'react-router-dom'
import { UserForm } from '../components/UserForm'
import { useUser, useUpdateUser } from '../hooks/useUsers'
import type { CreateUserPayload } from '../types/user'

export function UserEdit() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { data, isLoading } = useUser(id!)
  const mutation = useUpdateUser(id!)

  async function handleSubmit(formData: CreateUserPayload) {
    await mutation.mutateAsync({ firstName: formData.firstName, lastName: formData.lastName })
    navigate(`/users/${id}`)
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-2 border-neutral-200 border-t-primary-600" />
      </div>
    )
  }

  if (!data) return null

  const defaultValues: Partial<CreateUserPayload> = {
    firstName: data.firstName,
    lastName: data.lastName,
  }

  return (
    <div className="max-w-2xl space-y-6">
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
        <h1 className="text-2xl font-bold text-neutral-900">Editar Usuário</h1>
        <p className="text-sm text-neutral-500">{data.fullName} · {data.email}</p>
      </div>

      {mutation.error && (
        <div className="rounded-xl bg-danger-50 border border-danger-100 p-4">
          <p className="text-sm font-medium text-danger-700">{mutation.error.message}</p>
        </div>
      )}

      <div className="rounded-xl bg-white border border-neutral-200 shadow-card p-6">
        <UserForm
          defaultValues={defaultValues}
          onSubmit={handleSubmit}
          isLoading={mutation.isPending}
          submitLabel="Salvar Alterações"
          showEmail={false}
          showPassword={false}
          showRoles={false}
        />
      </div>
    </div>
  )
}
