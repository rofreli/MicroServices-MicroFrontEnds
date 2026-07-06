/**
 * Environment for the full-journey tests.
 *
 * The journeys drive the real stack through the BFF gateway (the single entry
 * point). Everything can be overridden via environment variables so the same
 * suite runs against local docker compose or another environment.
 */
export const GATEWAY_URL = process.env.E2E_GATEWAY_URL ?? 'http://localhost:5002'

export const CREDENTIALS = {
  email: process.env.E2E_USER_EMAIL ?? 'rodrigofreitas218@gmail.com',
  password: process.env.E2E_USER_PASSWORD ?? 'Admin@1234',
}

/** True when the BFF gateway is answering (i.e. `docker compose up` was run). */
export async function stackIsUp(): Promise<boolean> {
  try {
    const res = await fetch(`${GATEWAY_URL}/account/login`, {
      signal: AbortSignal.timeout(3_000),
    })
    return res.ok
  } catch {
    return false
  }
}

export const STACK_DOWN_MESSAGE =
  `Stack indisponível em ${GATEWAY_URL} — suba com "docker compose up -d" na raiz do repositório.`
