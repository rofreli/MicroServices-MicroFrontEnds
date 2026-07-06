import { defineConfig, devices } from '@playwright/test'

/**
 * Playwright configuration for the platform's end-to-end tests.
 *
 * By default the suite runs against the shell (the app's single UI entry point) at
 * http://localhost:3000. Override with E2E_BASE_URL to point at another environment.
 *
 * The `webServer` block starts the shell dev server automatically and reuses one that
 * is already running locally. Full user journeys additionally need the BFF gateway and
 * the domain services up (`docker compose up`); the bundled smoke test is designed to
 * pass against the shell alone.
 */
const baseURL = process.env.E2E_BASE_URL ?? 'http://localhost:3000'

export default defineConfig({
  testDir: './tests',
  // Full journeys go through the real OAuth flow on every test; give them room.
  timeout: 60_000,
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: process.env.CI ? [['github'], ['html', { open: 'never' }]] : 'html',

  use: {
    baseURL,
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
  },

  projects: [
    { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
    { name: 'firefox', use: { ...devices['Desktop Firefox'] } },
    // WebKit needs extra Linux system libraries. Enable it after installing them:
    //   npx playwright install-deps webkit   (or: sudo npx playwright install-deps)
    // { name: 'webkit', use: { ...devices['Desktop Safari'] } },
  ],

  // Auto-start the shell dev server unless one is already running (or E2E_BASE_URL points elsewhere).
  webServer: process.env.E2E_BASE_URL
    ? undefined
    : {
        command: 'npm --prefix ../shell run dev',
        url: 'http://localhost:3000',
        reuseExistingServer: !process.env.CI,
        timeout: 120_000,
      },
})
