import { apiClient } from './client'
import type {
  AddPermissionPayload,
  CreateUserPayload,
  PaginatedResult,
  RemovePermissionPayload,
  UpdateUserPayload,
  User,
  UserSummary,
} from '../types/user'

export const usersApi = {
  getAll: (page = 1, pageSize = 20) =>
    apiClient.get<PaginatedResult<UserSummary>>('/users', { params: { page, pageSize } }).then(r => r.data),

  getById: (id: string) =>
    apiClient.get<User>(`/users/${id}`).then(r => r.data),

  create: (payload: CreateUserPayload) =>
    apiClient.post<User>('/users', payload).then(r => r.data),

  update: (id: string, payload: UpdateUserPayload) =>
    apiClient.put<User>(`/users/${id}`, payload).then(r => r.data),

  deactivate: (id: string) =>
    apiClient.patch(`/users/${id}/deactivate`),

  activate: (id: string) =>
    apiClient.patch(`/users/${id}/activate`),

  addPermission: (id: string, payload: AddPermissionPayload) =>
    apiClient.post<User>(`/users/${id}/permissions`, payload).then(r => r.data),

  removePermission: (id: string, payload: RemovePermissionPayload) =>
    apiClient.delete<User>(`/users/${id}/permissions`, { data: payload }).then(r => r.data),
}
