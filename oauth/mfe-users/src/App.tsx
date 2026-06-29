import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { UsersRoutes } from './routes'
import './index.css'

const queryClient = new QueryClient({ defaultOptions: { queries: { staleTime: 60_000, retry: 1 } } })

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <div className="mfe-users">
        <UsersRoutes />
      </div>
    </QueryClientProvider>
  )
}
