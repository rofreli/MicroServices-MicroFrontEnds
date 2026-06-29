import axios from 'axios'

const API_BASE = import.meta.env.VITE_OAUTH_API_URL ?? 'http://localhost:5001'

export const apiClient = axios.create({
  baseURL: `${API_BASE}/api/v1`,
  headers: { 'Content-Type': 'application/json' },
})

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('access_token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

apiClient.interceptors.response.use(
  (res) => res,
  (err) => Promise.reject(new Error(err.response?.data?.message ?? err.message)),
)
