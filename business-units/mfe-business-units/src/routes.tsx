import { Route, Routes } from 'react-router-dom'
import { BusinessesList } from './pages/BusinessesList'
import { BusinessCreate } from './pages/BusinessCreate'
import { BusinessDetail } from './pages/BusinessDetail'
import { BusinessEdit } from './pages/BusinessEdit'

// Empresa (Business) is the top-level entity; Business Units are managed inside a
// Business's detail page (which supplies the required businessId).
export function BusinessUnitsRoutes() {
  return (
    <Routes>
      <Route index element={<BusinessesList />} />
      <Route path="new" element={<BusinessCreate />} />
      <Route path=":id" element={<BusinessDetail />} />
      <Route path=":id/edit" element={<BusinessEdit />} />
    </Routes>
  )
}
