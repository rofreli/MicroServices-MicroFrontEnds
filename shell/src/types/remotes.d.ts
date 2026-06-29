declare module 'mfeBusinessUnits/App' {
  import type { ComponentType } from 'react'
  const App: ComponentType<{ basename?: string }>
  export default App
}

declare module 'mfeBusinessUnits/routes' {
  import type { ComponentType } from 'react'
  export const BusinessUnitsRoutes: ComponentType
}

declare module 'mfeUsers/App' {
  import type { ComponentType } from 'react'
  const App: ComponentType
  export default App
}
