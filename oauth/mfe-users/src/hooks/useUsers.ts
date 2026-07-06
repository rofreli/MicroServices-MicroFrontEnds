import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { usersApi } from '../api/users'
import type {
  AddPermissionPayload,
  CreateUserPayload,
  RemovePermissionPayload,
  UpdateUserPayload,
} from '../types/user'

const KEYS = {
  all: ['users'] as const,
  list: (page: number, pageSize: number) =>
    ['users', 'list', page, pageSize] as const,
  detail: (id: string) => ['users', 'detail', id] as const,
}

export function useUsers(page = 1, pageSize = 20) {
  return useQuery({
    queryKey: KEYS.list(page, pageSize),
    queryFn: () => usersApi.getAll(page, pageSize),
  })
}

export function useUser(id: string) {
  return useQuery({
    queryKey: KEYS.detail(id),
    queryFn: () => usersApi.getById(id),
    enabled: !!id,
  })
}

export function useCreateUser() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (payload: CreateUserPayload) => usersApi.create(payload),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.all }),
  })
}

export function useUpdateUser(id: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (payload: UpdateUserPayload) => usersApi.update(id, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.all })
      qc.invalidateQueries({ queryKey: KEYS.detail(id) })
    },
  })
}

export function useDeactivateUser() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => usersApi.deactivate(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.all }),
  })
}

export function useActivateUser() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => usersApi.activate(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.all }),
  })
}

export function useAddPermission(id: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (payload: AddPermissionPayload) => usersApi.addPermission(id, payload),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.detail(id) }),
  })
}

export function useRemovePermission(id: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (payload: RemovePermissionPayload) => usersApi.removePermission(id, payload),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.detail(id) }),
  })
}
