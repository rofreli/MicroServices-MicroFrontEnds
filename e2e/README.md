# E2E — Playwright

End-to-end tests for the platform, driven through the **shell** (the single UI entry point).

## Install

```bash
cd e2e
npm install
npm run install:browsers   # downloads the browser binaries
```

Default projects are **Chromium** and **Firefox**. **WebKit** is opt-in (needs extra
Linux system libraries) — install them and uncomment the `webkit` project in
`playwright.config.ts`:

```bash
npx playwright install-deps webkit   # or: sudo npx playwright install-deps
```

## Run

```bash
npm test              # all browsers, HTML report
npm run test:chromium # Chromium only (fastest)
npm run test:ui       # interactive UI mode
npm run report        # open the last HTML report
```

By default Playwright auto-starts the shell dev server on `http://localhost:3000`
(reusing one already running). Point at another environment with:

```bash
E2E_BASE_URL=http://localhost:3000 npm test
```

## Suites

| Suite | Command | Requires |
|---|---|---|
| Smoke (`tests/smoke.spec.ts`) | `npm run test:smoke` | shell only |
| Journeys (`tests/journeys/`) | `npm run test:journeys` | full stack (`docker compose up -d`) |

The **journeys** replicate the exploratory flows validated with the Playwright MCP:

- `auth.spec.ts` — login válido, credenciais inválidas, logout real (sem re-login
  silencioso pelo cookie do gateway).
- `dashboard.spec.ts` — contadores agregados pelo BFF (`GET /api/v1/dashboard`).
- `businesses.spec.ts` — Empresa: criar → listar → detalhar → editar → excluir, com
  Unidade de Negócio aninhada (criar/excluir). CNPJs válidos são gerados por execução.
- `users.spec.ts` — Usuário: listar (badge Super Admin do seed) → criar → detalhar →
  permissões (adicionar, persistência após reload, remover) → inativar/reativar.

If the gateway is not reachable the journey suites **skip** themselves with an
explanatory message instead of failing.

### Configuration (env vars)

| Variable | Default |
|---|---|
| `E2E_BASE_URL` | `http://localhost:3000` (shell) |
| `E2E_GATEWAY_URL` | `http://localhost:5002` (BFF) |
| `E2E_USER_EMAIL` | seed super-admin email |
| `E2E_USER_PASSWORD` | seed super-admin password |

## Notes

- The smoke test intercepts the OAuth redirect so it passes against the shell alone —
  no backend required.
- Everything reaches the backend only through the **BFF gateway** (`:5002`).
- The journey specs create uniquely-named records (timestamp + random suffix) and
  delete what they create, so they are safe to re-run and to run on both browsers
  in parallel.
