export type ContactType = 'Primary' | 'Secondary' | 'Technical' | 'Commercial'

export interface Address {
  street: string
  number: string
  complement?: string
  district: string
  city: string
  state: string
  zipCode: string
  country: string
}

export interface Contact {
  id: string
  name: string
  email: string
  phone: string
  type: ContactType
}

export interface BusinessUnit {
  id: string
  razaoSocial: string
  nomeFantasia: string
  cnpj: string
  address?: Address
  contacts: Contact[]
  createdAt: string
  updatedAt?: string
}

export interface BusinessUnitSummary {
  id: string
  razaoSocial: string
  nomeFantasia: string
  cnpj: string
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

export interface CreateBusinessUnitPayload {
  razaoSocial: string
  nomeFantasia: string
  cnpj: string
  address?: Omit<Address, 'country'> & { country?: string }
  contacts?: Omit<Contact, 'id'>[]
}

export interface UpdateBusinessUnitPayload {
  razaoSocial: string
  nomeFantasia: string
  address?: Omit<Address, 'country'> & { country?: string }
  contacts?: Omit<Contact, 'id'>[]
}
