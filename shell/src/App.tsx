import { Route, Routes } from 'react-router-dom'
import { AuthProvider } from './auth/AuthContext'
import { Layout } from './components/Layout'
import { Dashboard } from './pages/Dashboard'
import { Login } from './pages/Login'
import { AuthCallback } from './pages/AuthCallback'
import { ProtectedRoute } from './components/ProtectedRoute'
import { MicroFrontendLoader } from './components/MicroFrontendLoader'

function BusinessUnitsMFE() {
  return <MicroFrontendLoader loader={() => import('mfeBusinessUnits/App')} />
}

function UsersMFE() {
  return <MicroFrontendLoader loader={() => import('mfeUsers/App')} />
}

export default function App() {
  return (
    <AuthProvider>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/auth/callback" element={<AuthCallback />} />
        <Route element={<ProtectedRoute><Layout /></ProtectedRoute>}>
          <Route index element={<Dashboard />} />
          <Route path="/business-units/*" element={<BusinessUnitsMFE />} />
          <Route path="/users/*" element={<UsersMFE />} />
        </Route>
      </Routes>
    </AuthProvider>
  )
}
