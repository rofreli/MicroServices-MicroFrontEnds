import { test, expect, type Page } from '@playwright/test'
import { goToSection, login } from '../helpers/auth'
import { CREDENTIALS, stackIsUp, STACK_DOWN_MESSAGE } from '../helpers/env'

/**
 * Journey: Usuários (MFE users via BFF).
 * Cobre listar → criar → detalhar → permissões (adicionar/persistir/remover)
 * → inativar/ativar.
 *
 * Serial: os passos compartilham o usuário criado no segundo teste.
 */
test.describe.serial('usuários e permissões', () => {
  const run = `${Date.now()}-${Math.floor(Math.random() * 1e4)}`
  const novoUsuario = {
    firstName: 'Maria',
    lastName: `E2E ${run}`,
    email: `maria.e2e.${run}@example.com`,
    password: 'Senha@1234',
  }
  const permissao = { businessId: `biz-e2e-${run}` }

  test.beforeAll(async () => {
    test.skip(!(await stackIsUp()), STACK_DOWN_MESSAGE)
  })

  async function openUserDetail(page: Page): Promise<void> {
    await login(page)
    await goToSection(page, '/users')
    await page.getByRole('row').filter({ hasText: novoUsuario.email }).click()
    await expect(
      page.getByRole('heading', { name: `${novoUsuario.firstName} ${novoUsuario.lastName}` }),
    ).toBeVisible()
  }

  test('lista usuários com o admin seed marcado como Super Admin', async ({ page }) => {
    await login(page)
    await goToSection(page, '/users')

    await expect(page.getByRole('heading', { name: 'Usuários' })).toBeVisible()
    const adminRow = page.getByRole('row').filter({ hasText: CREDENTIALS.email })
    await expect(adminRow).toBeVisible()
    await expect(adminRow.getByText('Super Admin')).toBeVisible()
  })

  test('cria um usuário', async ({ page }) => {
    await login(page)
    await goToSection(page, '/users')

    await page.getByRole('link', { name: 'Novo Usuário' }).click()
    await expect(page.getByRole('heading', { name: 'Novo Usuário' })).toBeVisible()

    await page.getByPlaceholder('João').fill(novoUsuario.firstName)
    await page.getByPlaceholder('Silva').fill(novoUsuario.lastName)
    await page.getByPlaceholder('joao@empresa.com').fill(novoUsuario.email)
    await page.getByPlaceholder('••••••••').fill(novoUsuario.password)
    await page.getByRole('button', { name: 'Criar Usuário' }).click()

    // Success navigates straight to the new user's detail page.
    await expect(
      page.getByRole('heading', { name: `${novoUsuario.firstName} ${novoUsuario.lastName}` }),
    ).toBeVisible({ timeout: 15_000 })

    // And the list shows it as a standard (non-admin), active account.
    await goToSection(page, '/users')
    const row = page.getByRole('row').filter({ hasText: novoUsuario.email })
    await expect(row).toBeVisible()
    await expect(row.getByText('Padrão')).toBeVisible()
    await expect(row.getByText('Ativo', { exact: true })).toBeVisible()
  })

  test('edita o usuário', async ({ page }) => {
    await openUserDetail(page)

    await page.getByRole('link', { name: 'Editar' }).click()
    await expect(page.getByRole('heading', { name: 'Editar Usuário' })).toBeVisible()

    await page.getByPlaceholder('Silva').fill(`${novoUsuario.lastName} Editada`)
    await page.getByRole('button', { name: 'Salvar Alterações' }).click()

    // Success navigates back to the detail page with the updated name.
    await expect(
      page.getByRole('heading', {
        name: `${novoUsuario.firstName} ${novoUsuario.lastName} Editada`,
      }),
    ).toBeVisible({ timeout: 15_000 })

    // Restore the original name so the remaining serial steps keep matching.
    await page.getByRole('link', { name: 'Editar' }).click()
    await page.getByPlaceholder('Silva').fill(novoUsuario.lastName)
    await page.getByRole('button', { name: 'Salvar Alterações' }).click()
    await expect(
      page.getByRole('heading', { name: `${novoUsuario.firstName} ${novoUsuario.lastName}` }),
    ).toBeVisible({ timeout: 15_000 })
  })

  test('adiciona uma permissão e ela persiste após recarregar', async ({ page }) => {
    await openUserDetail(page)
    await expect(page.getByText('Nenhuma permissão atribuída.')).toBeVisible()

    // Módulo/Papel keep their defaults (Business / BusinessAdmin).
    await page.getByPlaceholder('business id').fill(permissao.businessId)
    await page.getByRole('button', { name: 'Adicionar', exact: true }).click()

    // Badges are class-scoped so the <option> elements of the selects don't collide.
    await expect(
      page.locator('span.bg-primary-50', { hasText: 'Business' }),
    ).toBeVisible({ timeout: 15_000 })
    await expect(page.locator('span.bg-blue-50', { hasText: 'BusinessAdmin' })).toBeVisible()
    await expect(page.getByText(permissao.businessId)).toBeVisible()

    // Regression check: permissions must survive a refetch from MongoDB
    // (they used to serialize but read back empty).
    await page.reload()
    await expect(page.getByText(permissao.businessId)).toBeVisible({ timeout: 15_000 })
  })

  test('remove a permissão', async ({ page }) => {
    await openUserDetail(page)
    await expect(page.getByText(permissao.businessId)).toBeVisible()

    await page.getByRole('button', { name: 'Remover' }).click()

    await expect(page.getByText('Nenhuma permissão atribuída.')).toBeVisible({ timeout: 15_000 })
    await page.reload()
    await expect(page.getByText('Nenhuma permissão atribuída.')).toBeVisible({ timeout: 15_000 })
  })

  test('inativa e reativa o usuário', async ({ page }) => {
    await openUserDetail(page)

    await page.getByRole('button', { name: 'Inativar' }).click()
    await expect(page.getByText('Inativo', { exact: true })).toBeVisible({ timeout: 15_000 })

    await page.getByRole('button', { name: 'Ativar', exact: true }).click()
    await expect(page.getByText('Ativo', { exact: true })).toBeVisible({ timeout: 15_000 })
  })
})
