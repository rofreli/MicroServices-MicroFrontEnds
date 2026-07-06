import { expect, type Page } from '@playwright/test'
import { CREDENTIALS } from './env'

/**
 * Signs in through the real OAuth flow:
 * shell → /connect/authorize (gateway) → /account/login → back to the shell dashboard.
 */
export async function login(
  page: Page,
  { email, password } = CREDENTIALS,
): Promise<void> {
  await page.goto('/')

  // Unauthenticated visits are redirected by the auth guard to the gateway login page.
  await page.waitForURL('**/account/login**', { timeout: 20_000 })
  await page.locator('input[name="email"]').fill(email)
  await page.locator('input[name="password"]').fill(password)
  await page.getByRole('button', { name: 'Entrar', exact: true }).click()

  // Back on the shell: the dashboard greeting proves the token exchange worked.
  await expect(
    page.getByText('Bem-vindo à plataforma. Aqui está um resumo do sistema.'),
  ).toBeVisible({ timeout: 20_000 })
}

/** Clicks the header logout button and waits for the OAuth login page. */
export async function logout(page: Page): Promise<void> {
  await page.getByTitle('Sair da conta').click()
  await page.waitForURL('**/account/login**', { timeout: 20_000 })
  await expect(page.getByText('Entre na sua conta')).toBeVisible()
}

/** Navigates to an MFE section via its sidebar link (avoids relying on SPA fallback). */
export async function goToSection(page: Page, href: '/business-units' | '/users'): Promise<void> {
  await page.locator(`a[href="${href}"]`).first().click()
}
