export interface Permission {
  businessId: string
  businessUnitId?: string | null
  module: string
  function?: string | null
  role: string
}

export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  fullName: string
  isActive: boolean
  isSuperAdmin: boolean
  permissions: Permission[]
  externalProviders: string[]
  createdAt: string
  updatedAt?: string
}

export interface UserSummary {
  id: string
  email: string
  fullName: string
  isActive: boolean
  isSuperAdmin: boolean
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
}

export interface UpdateUserPayload {
  firstName: string
  lastName: string
}

// Mirrors the OAuth domain constants (OAuthModules / OAuthRoles / OAuthFunctions).
export const MODULES = ['Business', 'BusinessUnit', 'Users'] as const
export const ROLES = [
  'BusinessAdmin',
  'BusinessUnitAdmin',
  'ModuleAdmin',
  'Manager',
  'Reader',
  'Writer',
] as const
export const FUNCTIONS = ['InviteUser'] as const

export interface AddPermissionPayload {
  businessId: string
  businessUnitId?: string | null
  module: string
  function?: string | null
  role: string
}

export interface RemovePermissionPayload {
  businessId: string
  businessUnitId?: string | null
  module: string
  function?: string | null
}
