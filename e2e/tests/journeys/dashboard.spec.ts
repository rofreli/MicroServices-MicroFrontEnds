import { test, expect } from '@playwright/test'
import { login } from '../helpers/auth'
import { stackIsUp, STACK_DOWN_MESSAGE } from '../helpers/env'

/**
 * Journey: dashboard agregado via BFF (`GET /api/v1/dashboard`).
 */
test.describe('dashboard', () => {
  test.beforeAll(async () => {
    test.skip(!(await stackIsUp()), STACK_DOWN_MESSAGE)
  })

  test('exibe contadores reais vindos da agregação do BFF', async ({ page }) => {
    const dashboardResponse = page.waitForResponse(
      (r) => r.url().includes('/api/v1/dashboard') && r.request().method() === 'GET',
      { timeout: 30_000 },
    )

    await login(page)

    const response = await dashboardResponse
    expect(response.ok()).toBeTruthy()

    const counts = await response.json()
    expect(typeof counts.businessCount).toBe('number')
    expect(typeof counts.businessUnitCount).toBe('number')
    expect(typeof counts.userCount).toBe('number')
    expect(counts.userCount).toBeGreaterThanOrEqual(1) // at least the seed admin

    // The three stat tiles are rendered and none is left as the "—" placeholder.
    for (const title of ['Empresas', 'Unidades de Negócio', 'Usuários']) {
      await expect(page.getByText(title, { exact: true }).first()).toBeVisible()
    }
    await expect(page.locator('p.text-xl', { hasText: '—' })).toHaveCount(0)
  })
})
