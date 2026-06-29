import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { businessUnitsApi } from '../api/businessUnits'
import type {
  CreateBusinessUnitPayload,
  UpdateBusinessUnitPayload,
} from '../types/businessUnit'

const KEYS = {
  all: ['business-units'] as const,
  list: (page: number, pageSize: number) =>
    ['business-units', 'list', page, pageSize] as const,
  detail: (id: string) => ['business-units', 'detail', id] as const,
}

export function useBusinessUnits(page = 1, pageSize = 20) {
  return useQuery({
    queryKey: KEYS.list(page, pageSize),
    queryFn: () => businessUnitsApi.getAll(page, pageSize),
  })
}

export function useBusinessUnit(id: string) {
  return useQuery({
    queryKey: KEYS.detail(id),
    queryFn: () => businessUnitsApi.getById(id),
    enabled: !!id,
  })
}

export function useCreateBusinessUnit() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (payload: CreateBusinessUnitPayload) =>
      businessUnitsApi.create(payload),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.all }),
  })
}

export function useUpdateBusinessUnit(id: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (payload: UpdateBusinessUnitPayload) =>
      businessUnitsApi.update(id, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: KEYS.all })
      qc.invalidateQueries({ queryKey: KEYS.detail(id) })
    },
  })
}

export function useDeleteBusinessUnit() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => businessUnitsApi.remove(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: KEYS.all }),
  })
}
