import { test, expect } from '@playwright/test'

/**
 * Basic smoke test for the shell — the single UI entry point that hosts the MFEs.
 * No backend stack required: an unauthenticated visit auto-redirects to the OAuth
 * provider (via the BFF gateway), and we assert the shell got that far.
 */
test.describe('shell smoke', () => {
  test('serves the shell index html', async ({ page }) => {
    const response = await page.goto('/')

    expect(response?.status()).toBeLessThan(400)
    const html = (await response?.text()) ?? ''
    expect(html).toContain('id="root"')
    expect(html).toMatch(/<title>[^<]*Platform/)
  })

  test('boots, mounts the SPA and initiates the OAuth login flow', async ({ page }) => {
    // The app redirects the whole page to /connect/authorize; capturing that request
    // proves React mounted, the router + auth guard ran, and the gateway URL is wired.
    const authorizeAttempt = page.waitForRequest('**/connect/authorize**', { timeout: 15_000 })
    await page.route('**/connect/authorize**', (route) => route.abort())

    await page.goto('/')

    const request = await authorizeAttempt
    expect(request.url()).toContain('/connect/authorize')
  })
})
