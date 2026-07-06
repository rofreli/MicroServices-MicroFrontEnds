import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { businessesApi } from '../api/businesses'
import type {
  CreateBusinessPayload,
  CreateBusinessUnitInBusinessPayload,
  UpdateBusinessPayload,
} from '../types/business'

const KEYS = {
  all: ['businesses'] as const,
  list: (page: number, pageSize: number) => ['businesses', 'list', page, pageSize] as const,
  detail: (id: string) => ['businesses', 'detail', id] as const,
  units: (id: string) => ['businesses', id, 'units'] as const,
}

export function useBusinesses(page = 1, pageSize = 20) {
  return useQuery({
    queryKey: KEYS.list(page, pageSize),
    queryFn: () => businessesApi.getAll(page, pageSize),
  })
}

export function useBusiness(id: string) {
  return useQuery({
    queryKey: KEYS.detail(id),
    queryFn: () => businessesApi.getById(id),
    enabled: !!id,
  })
}

export function useBusinessUnits(businessId: string) {
  return useQuery({
    queryKey: KEYS.units(businessId),
    queryFn: () => businessesApi.getUnits(businessId),
    enabled: !!businessId,
  })
}

export function useCreateBusiness() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (payload: CreateBusinessPayload) => businessesApi.create(payload),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.all }),
  })
}

export function useUpdateBusiness(id: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (payload: UpdateBusinessPayload) => businessesApi.update(id, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.all })
      qc.invalidateQueries({ queryKey: KEYS.detail(id) })
    },
  })
}

export function useDeleteBusiness() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => businessesApi.remove(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.all }),
  })
}

export function useCreateUnit(businessId: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (payload: CreateBusinessUnitInBusinessPayload) =>
      businessesApi.createUnit(businessId, payload),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.units(businessId) }),
  })
}

export function useDeleteUnit(businessId: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (unitId: string) => businessesApi.removeUnit(unitId),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.units(businessId) }),
  })
}
