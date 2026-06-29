import { Route, Routes } from 'react-router-dom'
import { UsersList } from './pages/UsersList'
import { UserCreate } from './pages/UserCreate'
import { UserEdit } from './pages/UserEdit'
import { UserDetail } from './pages/UserDetail'

export function UsersRoutes() {
  return (
    <Routes>
      <Route index element={<UsersList />} />
      <Route path="new" element={<UserCreate />} />
      <Route path=":id" element={<UserDetail />} />
      <Route path=":id/edit" element={<UserEdit />} />
    </Routes>
  )
}
