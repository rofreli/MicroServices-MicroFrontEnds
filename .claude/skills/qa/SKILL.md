---
name: qa
description: Estratégia e execução de QA do monorepo — testes unitários (domínio/handlers), integração do BFF (WebApplicationFactory), e2e Playwright (journeys) e exploratório via Playwright MCP. Use ao criar/alterar testes, validar fluxos no navegador ou investigar regressões.
---

# Skill: QA

Como o monorepo `/home/rodrigo/pessoal/repos/` é testado, camada por camada. Para arquitetura,
use a skill `arquitetura`; para scaffolding de domínio, o `SKILL.md` da raiz.

---

## 1. Pirâmide de testes

| Camada | Ferramenta | Onde | Comando |
|--------|-----------|------|---------|
| Unitário (Domain/Application) | xUnit 2.9.3 + FluentAssertions 7.2.2 + Moq 4.20.72 | `<domain>/backend/tests/` | `dotnet test <domain>/backend` |
| Integração (BFF) | `WebApplicationFactory<Program>` (`Mvc.Testing` 10.0.9) | `bff/backend/tests/` | `dotnet test bff/backend` |
| E2E (journeys) | Playwright `@playwright/test` | `e2e/tests/journeys/` | `cd e2e && npm run test:journeys` |
| Smoke (sem backend) | Playwright | `e2e/tests/smoke.spec.ts` | `cd e2e && npm run test:smoke` |
| Exploratório | Playwright MCP (browser real) | sessão interativa | ver §5 |

Projetos de teste .NET usam TFM **`net10.0`** (igual à produção) e `Microsoft.AspNetCore.Mvc.Testing`
**no mesmo major do runtime** — desalinhamento causa `PipeWriter does not implement UnflushedBytes`
(500 dentro da factory).

## 2. Testes unitários

**Domínio** — puramente in-memory, sem mocks. Testar invariantes: value objects (`Cnpj`, `Email`),
factories, guardas de estado (`Deactivate` duas vezes lança `DomainException`), regras de coleção
(`AddPermission` substitui a permissão de mesmo escopo).

**Handlers (Application)** — repositórios via `Mock<I...Repository>`; **AutoMapper real** montado
dos profiles:

```csharp
public static class MapperFactory
{
    public static IMapper Create() =>
        new MapperConfiguration(cfg => cfg.AddMaps(typeof(<Entity>MappingProfile).Assembly))
            .CreateMapper();
}
```

Cobrir caminhos felizes **e** de erro (duplicidade → `ConflictException`, ausência → `NotFoundException`).

## 3. Testes de integração do BFF

`Program.cs` termina com `public partial class Program { }`. A factory substitui **dois seams** e
exercita todo o resto de verdade:

- **Autenticação** → `TestAuthHandler` (esquema `Test`): autentica só com header `Authorization`;
  claims vêm de `X-Test-SuperAdmin` / `X-Test-Permissions`.
- **Transporte downstream** → `StubHttpMessageHandler` via `ConfigurePrimaryHttpMessageHandler(...)`
  em cada typed client, devolvendo JSON canônico (camelCase, igual ao dos domínios).

Casos essenciais: **401** sem token, **200** agregando, **403** sem acesso ao business, **404**
recurso inexistente, **502** quando um domínio cai (o stub lança `HttpRequestException`).

> Armadilha: stub que casa a query com `Contains("pageSize=1")` também casa `pageSize=100` —
> parsear a query (`HttpUtility.ParseQueryString`) e comparar o valor exato.

## 4. E2E — journeys Playwright (`e2e/`)

Cobrem os fluxos completos pelo navegador, **através do fluxo OAuth real** e do BFF:

| Spec | Fluxos |
|------|--------|
| `journeys/auth.spec.ts` | login válido, credenciais inválidas, logout real (regressão do re-login silencioso) |
| `journeys/dashboard.spec.ts` | contadores agregados do BFF (`/api/v1/dashboard`), sem tiles "—" |
| `journeys/businesses.spec.ts` | Empresa criar→listar→detalhar→editar→excluir + Unidade aninhada criar/excluir |
| `journeys/users.spec.ts` | listar (badge Super Admin), criar, editar, permissões (add→persistência pós-reload→remover), inativar/reativar |

```bash
cd e2e
npm run test:journeys                        # requer docker compose up -d
npm run test:journeys -- --project=chromium  # só Chromium (mais rápido)
```

Env: `E2E_BASE_URL` (shell, :3000), `E2E_GATEWAY_URL` (BFF, :5002), `E2E_USER_EMAIL`/`E2E_USER_PASSWORD`
(seed super admin). Se o gateway não responder, a suíte **se auto-pula** (`stackIsUp()` em
`tests/helpers/env.ts`) em vez de falhar.

### Convenções ao escrever um journey novo

1. **Serial por domínio** (`test.describe.serial`) — passos compartilham a entidade criada no
   primeiro teste; cada teste loga de novo (contexto novo = sessão isolada).
2. **Dados únicos por execução** — sufixo `${Date.now()}-${random}` em nomes/e-mails e
   `randomCnpj()` (`helpers/cnpj.ts`, dígitos verificadores válidos) para nunca colidir com o
   índice único; excluir o que criar. Assim a suíte roda em 2 browsers em paralelo.
3. **Seletores**: os labels dos MFEs **não têm `htmlFor`** — `getByLabel` não funciona; usar
   `getByPlaceholder`/`getByRole`. Badges que repetem texto de `<option>` de um `<select>`
   (ex.: "Business", "BusinessAdmin") precisam de escopo por classe
   (`span.bg-primary-50`) para não violar strict mode.
4. **`confirm()` de exclusão**: registrar `page.on('dialog', d => d.accept())` **antes** do click.
5. **Login/logout**: usar `helpers/auth.ts` (`login`, `logout`, `goToSection`) — nunca duplicar o
   fluxo OAuth nos specs. Navegar entre MFEs por `a[href="/..."]` (não depender de SPA fallback).
6. Esperar elementos de negócio (heading, linha da tabela), não `networkidle`.

## 5. Exploratório via Playwright MCP

Para investigar fluxos manualmente com o browser real (MCP `playwright` já configurado no
`.mcp.json` do repo, modo `--isolated`):

- **Credenciais seed:** `rodrigofreitas218@gmail.com` / `Admin@1234` (super admin).
- **Sessão limpa** (equivalente a anônimo): `browser_run_code_unsafe` →
  `page.context().clearCookies()` + limpar `localStorage`/`sessionStorage`.
- **Bundle MFE desatualizado após rebuild** (nginx serve `/assets` com `immutable`): limpar via
  CDP — `Network.clearBrowserCache` + `Network.setCacheDisabled` — e recarregar.
- Diagnóstico: `browser_network_requests` para ver o status real das chamadas ao BFF;
  `browser_console_messages` para erros de federation/CORS.

## 6. Armadilhas de QA

| Situação | Sintoma | Solução |
|----------|---------|---------|
| `Mvc.Testing` em major ≠ runtime | `PipeWriter ... UnflushedBytes` (500 na factory) | Casar major (net10 → 10.0.9) |
| Stub HTTP com `Contains("pageSize=1")` | casa `pageSize=100` → dados errados | Parsear a query e comparar exato |
| oauth-api reiniciado | 401 em tudo com token antigo | Tokens invalidados (DataProtection) — relogar |
| Gateway logo após `up -d` | 502 no `/account/login` → journeys se auto-pulam | Aguardar `/account/login` responder 200 antes de rodar |
| Rebuild de MFE não aparece | browser preso no bundle antigo (`immutable`) | Hard-refresh / limpar cache via CDP |
| CNPJ repetido entre execuções | 409/erro de índice único | `randomCnpj()` por execução |
| WebKit no Linux | falta de libs de sistema | `npx playwright install-deps webkit` e descomentar o project |

## 7. Regressões que valem teste (bugs históricos reais)

Ao tocar nessas áreas, garanta que o cenário segue coberto:

1. **Logout com re-login silencioso** — `setState` antes do redirect abortava `/connect/logout`;
   coberto por `auth.spec.ts` (revisita `/` pós-logout e exige tela de login).
2. **Permissões que não persistem** — backing field `readonly` serializa mas desserializa vazio
   no MongoDB; coberto por `users.spec.ts` (reload após adicionar permissão).
3. **PUT devolvendo 400 sempre** — `[FromBody] UpdateXxxCommand` exige `Id` que o body não traz;
   o body deve ser um record próprio sem `Id`. Coberto pelos testes de *editar* empresa/usuário.
4. **Introspection audience** — cliente de introspection do BFF fora das audiences → 401 geral;
   qualquer journey autenticado pega isso imediatamente.
5. **`EnsureIndexes` derrubando a API** — índice legado com nome diferente → 500 em tudo;
   criação de índice deve ser tolerante (`TryCreateIndex`).
6. **Body de POST perdido no gateway** (h2c / stream consumido) — login e `/connect/token` param;
   qualquer journey pega no primeiro login.
