import { test, expect, type Page } from '@playwright/test'
import { goToSection, login } from '../helpers/auth'
import { randomCnpj } from '../helpers/cnpj'
import { stackIsUp, STACK_DOWN_MESSAGE } from '../helpers/env'

/**
 * Journey: Empresas + Unidades de Negócio (MFE business-units via BFF).
 * Cobre criar → listar → detalhar → unidade aninhada (criar/excluir) → editar → excluir.
 *
 * Serial: os passos compartilham a empresa criada no primeiro teste.
 */
test.describe.serial('empresas e unidades de negócio', () => {
  const run = `${Date.now()}-${Math.floor(Math.random() * 1e4)}`
  const empresa = {
    razaoSocial: `E2E Empresa ${run} LTDA`,
    nomeFantasia: `E2E Empresa ${run}`,
    nomeFantasiaEditado: `E2E Empresa ${run} (editada)`,
    cnpj: randomCnpj(),
  }
  const unidade = {
    razaoSocial: `E2E Filial ${run} LTDA`,
    nomeFantasia: `E2E Filial ${run}`,
    cnpj: randomCnpj(),
  }

  test.beforeAll(async () => {
    test.skip(!(await stackIsUp()), STACK_DOWN_MESSAGE)
  })

  async function openBusinessDetail(page: Page): Promise<void> {
    await login(page)
    await goToSection(page, '/business-units')
    await page.getByRole('row').filter({ hasText: empresa.razaoSocial }).click()
    await expect(
      page.getByRole('heading', { name: new RegExp(`E2E Empresa ${run}`) }),
    ).toBeVisible()
  }

  test('cria uma empresa', async ({ page }) => {
    await login(page)
    await goToSection(page, '/business-units')
    await expect(page.getByRole('heading', { name: 'Empresas' })).toBeVisible()

    await page.getByRole('link', { name: '+ Nova Empresa' }).click()
    await expect(page.getByRole('heading', { name: 'Nova Empresa' })).toBeVisible()

    await page.getByPlaceholder('Acme Comércio LTDA').fill(empresa.razaoSocial)
    await page.getByPlaceholder('Acme', { exact: true }).fill(empresa.nomeFantasia)
    await page.getByPlaceholder('11.222.333/0001-81').fill(empresa.cnpj)
    await page.getByRole('button', { name: 'Criar Empresa' }).click()

    // Success navigates straight to the detail page.
    await expect(page.getByRole('heading', { name: empresa.razaoSocial })).toBeVisible({
      timeout: 15_000,
    })
    await expect(page).toHaveURL(/\/business-units\/[^/]+$/)
  })

  test('lista a empresa criada com status Ativa', async ({ page }) => {
    await login(page)
    await goToSection(page, '/business-units')

    const row = page.getByRole('row').filter({ hasText: empresa.razaoSocial })
    await expect(row).toBeVisible()
    await expect(row.getByText('Ativa')).toBeVisible()
  })

  test('adiciona uma unidade de negócio dentro da empresa', async ({ page }) => {
    await openBusinessDetail(page)

    await page.getByPlaceholder('Filial SP LTDA').fill(unidade.razaoSocial)
    await page.getByPlaceholder('Filial SP', { exact: true }).fill(unidade.nomeFantasia)
    await page.getByPlaceholder('11.222.333/0001-81').fill(unidade.cnpj)
    await page.getByRole('button', { name: 'Adicionar Unidade' }).click()

    await expect(page.getByText(unidade.razaoSocial)).toBeVisible({ timeout: 15_000 })
    await expect(page.getByText('Unidades de Negócio (1)')).toBeVisible()
  })

  test('exclui a unidade de negócio', async ({ page }) => {
    await openBusinessDetail(page)
    await expect(page.getByText(unidade.razaoSocial)).toBeVisible()

    // The detail page has a single "Excluir" button — the one on the unit row.
    page.on('dialog', (dialog) => dialog.accept())
    await page.getByRole('button', { name: 'Excluir' }).click()

    await expect(page.getByText('Nenhuma unidade cadastrada nesta empresa.')).toBeVisible({
      timeout: 15_000,
    })
  })

  test('edita a empresa', async ({ page }) => {
    await openBusinessDetail(page)

    await page.getByRole('link', { name: 'Editar' }).click()
    await expect(page.getByRole('heading', { name: 'Editar Empresa' })).toBeVisible()

    // Second input of the form is "Nome Fantasia" (fields have no placeholder here).
    const nomeFantasia = page.locator('form input').nth(1)
    await nomeFantasia.fill(empresa.nomeFantasiaEditado)
    await page.getByRole('button', { name: 'Salvar Alterações' }).click()

    await expect(page.getByText(empresa.nomeFantasiaEditado)).toBeVisible({ timeout: 15_000 })
  })

  test('exclui a empresa pela listagem', async ({ page }) => {
    await login(page)
    await goToSection(page, '/business-units')

    const row = page.getByRole('row').filter({ hasText: empresa.razaoSocial })
    await expect(row).toBeVisible()

    page.on('dialog', (dialog) => dialog.accept())
    await row.getByRole('button', { name: 'Excluir' }).click()

    await expect(row).toHaveCount(0, { timeout: 15_000 })
  })
})
