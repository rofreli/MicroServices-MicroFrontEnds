import { test, expect } from '@playwright/test'
import { login, logout } from '../helpers/auth'
import { CREDENTIALS, stackIsUp, STACK_DOWN_MESSAGE } from '../helpers/env'

/**
 * Journey: autenticação (login → logout).
 * Requer a stack completa (`docker compose up -d` na raiz).
 */
test.describe('autenticação', () => {
  test.beforeAll(async () => {
    test.skip(!(await stackIsUp()), STACK_DOWN_MESSAGE)
  })

  test('realiza login com credenciais válidas e chega ao dashboard', async ({ page }) => {
    await login(page)

    // Header shows the signed-in user's email.
    await expect(page.getByText(CREDENTIALS.email).first()).toBeVisible()
  })

  test('rejeita credenciais inválidas e permanece na tela de login', async ({ page }) => {
    await page.goto('/')
    await page.waitForURL('**/account/login**', { timeout: 20_000 })

    await page.locator('input[name="email"]').fill(CREDENTIALS.email)
    await page.locator('input[name="password"]').fill('senha-errada-123')
    await page.getByRole('button', { name: 'Entrar', exact: true }).click()

    // Still on the login page, with the error banner rendered.
    await expect(page).toHaveURL(/\/account\/login/)
    await expect(page.locator('.bg-red-50')).toBeVisible()
  })

  test('logout encerra a sessão de verdade (sem re-login silencioso)', async ({ page }) => {
    await login(page)
    await logout(page)

    // Regression check for the silent re-login bug: a fresh visit to the shell
    // must land on the login page again, not restore the session from the cookie.
    await page.goto('/')
    await page.waitForURL('**/account/login**', { timeout: 20_000 })
    await expect(page.getByText('Entre na sua conta')).toBeVisible()
  })
})
