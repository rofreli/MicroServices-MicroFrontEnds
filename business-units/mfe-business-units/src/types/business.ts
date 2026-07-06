export interface Business {
  id: string
  razaoSocial: string
  nomeFantasia: string
  cnpj: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface BusinessSummary {
  id: string
  razaoSocial: string
  nomeFantasia: string
  cnpj: string
  isActive: boolean
  createdAt: string
}

export interface CreateBusinessPayload {
  razaoSocial: string
  nomeFantasia: string
  cnpj: string
}

export interface UpdateBusinessPayload {
  razaoSocial: string
  nomeFantasia: string
}

export interface CreateBusinessUnitInBusinessPayload {
  razaoSocial: string
  nomeFantasia: string
  cnpj: string
}
