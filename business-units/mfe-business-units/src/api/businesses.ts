import { apiClient } from './client'
import type {
  Business,
  BusinessSummary,
  CreateBusinessPayload,
  CreateBusinessUnitInBusinessPayload,
  UpdateBusinessPayload,
} from '../types/business'
import type { BusinessUnit, BusinessUnitSummary, PaginatedResult } from '../types/businessUnit'

export const businessesApi = {
  getAll: (page = 1, pageSize = 20) =>
    apiClient
      .get<PaginatedResult<BusinessSummary>>('/businesses', { params: { page, pageSize } })
      .then((r) => r.data),

  getById: (id: string) =>
    apiClient.get<Business>(`/businesses/${id}`).then((r) => r.data),

  create: (payload: CreateBusinessPayload) =>
    apiClient.post<Business>('/businesses', payload).then((r) => r.data),

  update: (id: string, payload: UpdateBusinessPayload) =>
    apiClient.put<Business>(`/businesses/${id}`, payload).then((r) => r.data),

  remove: (id: string) =>
    apiClient.delete(`/businesses/${id}`).then((r) => r.data),

  // Business Units nested under a business
  getUnits: (businessId: string, page = 1, pageSize = 50) =>
    apiClient
      .get<PaginatedResult<BusinessUnitSummary>>(`/businesses/${businessId}/business-units`, {
        params: { page, pageSize },
      })
      .then((r) => r.data),

  createUnit: (businessId: string, payload: CreateBusinessUnitInBusinessPayload) =>
    apiClient
      .post<BusinessUnit>(`/businesses/${businessId}/business-units`, payload)
      .then((r) => r.data),

  removeUnit: (unitId: string) =>
    apiClient.delete(`/business-units/${unitId}`).then((r) => r.data),
}
