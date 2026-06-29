import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { BusinessUnitsRoutes } from './routes'
import './index.css'

const queryClient = new QueryClient({
  defaultOptions: { queries: { staleTime: 1000 * 60 * 5, retry: 1 } },
})

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <div className="mfe-business-units">
        <BusinessUnitsRoutes />
      </div>
    </QueryClientProvider>
  )
}
