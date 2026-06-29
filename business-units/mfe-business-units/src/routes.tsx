import { Route, Routes } from 'react-router-dom'
import { BusinessUnitsList } from './pages/BusinessUnitsList'
import { BusinessUnitCreate } from './pages/BusinessUnitCreate'
import { BusinessUnitEdit } from './pages/BusinessUnitEdit'
import { BusinessUnitDetail } from './pages/BusinessUnitDetail'

export function BusinessUnitsRoutes() {
  return (
    <Routes>
      <Route index element={<BusinessUnitsList />} />
      <Route path="new" element={<BusinessUnitCreate />} />
      <Route path=":id" element={<BusinessUnitDetail />} />
      <Route path=":id/edit" element={<BusinessUnitEdit />} />
    </Routes>
  )
}
