import { Suspense, lazy, ComponentType } from 'react'

interface Props {
  loader: () => Promise<{ default: ComponentType<Record<string, unknown>> }>
  props?: Record<string, unknown>
  fallback?: React.ReactNode
}

function ErrorFallback({ name }: { name: string }) {
  return (
    <div className="flex items-center justify-center h-64">
      <div className="text-center">
        <div className="flex h-12 w-12 items-center justify-center rounded-2xl bg-danger-50 mx-auto mb-3">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.75" strokeLinecap="round" strokeLinejoin="round" className="w-6 h-6 text-danger-500">
            <circle cx="12" cy="12" r="10" /><line x1="12" y1="8" x2="12" y2="12" /><line x1="12" y1="16" x2="12.01" y2="16" />
          </svg>
        </div>
        <p className="text-sm font-semibold text-neutral-700">Falha ao carregar: {name}</p>
        <p className="text-xs text-neutral-400 mt-1">Verifique se o micro-frontend está em execução.</p>
      </div>
    </div>
  )
}

export function MicroFrontendLoader({ loader, props = {}, fallback }: Props) {
  const Component = lazy(loader as Parameters<typeof lazy>[0])

  return (
    <Suspense
      fallback={
        fallback ?? (
          <div className="flex items-center justify-center h-64">
            <div className="animate-spin rounded-full h-8 w-8 border-2 border-neutral-200 border-t-primary-600" />
          </div>
        )
      }
    >
      <Component {...props} />
    </Suspense>
  )
}
