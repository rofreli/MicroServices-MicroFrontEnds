export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  fullName: string
  isActive: boolean
  roles: string[]
  externalProviders: string[]
  createdAt: string
  updatedAt?: string
}

export interface UserSummary {
  id: string
  email: string
  fullName: string
  isActive: boolean
  roles: string[]
  createdAt: string
}

export interface PaginatedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface CreateUserPayload {
  email: string
  firstName: string
  lastName: string
  password: string
  roles?: string[]
}

export interface UpdateUserPayload {
  firstName: string
  lastName: string
}

export const AVAILABLE_ROLES = ['ADMIN', 'MANAGER', 'VIEWER', 'DEVELOPER'] as const
