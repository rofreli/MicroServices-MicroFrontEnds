import axios from 'axios'

// All calls go through the BFF gateway — never directly to the inner services.
const API_BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5002'

export const apiClient = axios.create({
  baseURL: `${API_BASE_URL}/api/v1`,
  headers: { 'Content-Type': 'application/json' },
})

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('access_token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

apiClient.interceptors.response.use(
  (res) => res,
  (error) => {
    const message =
      error.response?.data?.message ?? error.message ?? 'Erro desconhecido'
    return Promise.reject(new Error(message))
  },
)
