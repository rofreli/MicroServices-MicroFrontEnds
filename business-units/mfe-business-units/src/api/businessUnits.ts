import { apiClient } from './client'
import type {
  BusinessUnit,
  BusinessUnitSummary,
  CreateBusinessUnitPayload,
  PaginatedResult,
  UpdateBusinessUnitPayload,
} from '../types/businessUnit'

export const businessUnitsApi = {
  getAll: (page = 1, pageSize = 20) =>
    apiClient
      .get<PaginatedResult<BusinessUnitSummary>>('/business-units', {
        params: { page, pageSize },
      })
      .then((r) => r.data),

  getById: (id: string) =>
    apiClient.get<BusinessUnit>(`/business-units/${id}`).then((r) => r.data),

  getByCnpj: (cnpj: string) =>
    apiClient
      .get<BusinessUnit>(`/business-units/cnpj/${cnpj}`)
      .then((r) => r.data),

  create: (payload: CreateBusinessUnitPayload) =>
    apiClient.post<BusinessUnit>('/business-units', payload).then((r) => r.data),

  update: (id: string, payload: UpdateBusinessUnitPayload) =>
    apiClient
      .put<BusinessUnit>(`/business-units/${id}`, payload)
      .then((r) => r.data),

  remove: (id: string) =>
    apiClient.delete(`/business-units/${id}`).then((r) => r.data),
}
