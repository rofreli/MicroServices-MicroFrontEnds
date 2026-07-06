---
name: arquitetura
description: Especificação em nível de arquitetura do monorepo — topologia, BFF como porta única, segurança (OIDC/introspection/token relay), isolamento de rede, mapa de portas, decisões registradas e template para especificar novas funcionalidades/domínios. Use ao projetar algo novo ou ao revisar decisões estruturais.
---

# Skill: Especificação de Arquitetura

Fonte de verdade arquitetural do monorepo `/home/rodrigo/pessoal/repos/`. Para o **passo a passo
de scaffolding** de um novo domínio, use o `SKILL.md` da raiz. Para **estratégia de testes**, use
a skill `qa`.

---

## 1. Visão do sistema

```
                        Browser
                           │
        ┌──────────────────┼───────────────────────┐
        │ :3000            │ :3001/:3002            │ :5002
   ┌────▼────┐      ┌──────▼───────┐        ┌───────▼────────┐
   │  Shell  │      │ MFEs (nginx) │        │   BFF (YARP)   │  ← ÚNICA porta de
   │ (host)  │      │ remoteEntry  │        │ gateway + agreg│    entrada de API
   └─────────┘      └──────────────┘        └───┬────────┬───┘
                                                │        │      rede privada
                                     ┌──────────▼──┐  ┌──▼──────────┐  (expose,
                                     │ business-   │  │  oauth-api  │  sem ports)
                                     │ units-api   │  │ (OpenIddict)│
                                     └──────┬──────┘  └──────┬──────┘
                                       ┌────▼────┐      ┌────▼────┐
                                       │ mongo   │      │ mongo   │
                                       └─────────┘      └─────────┘
```

- **Shell** (`shell/`): host de Module Federation; provê `<BrowserRouter>`, auth context e layout.
- **MFEs** (`<domain>/mfe-*`): React remotos; publicados no host apenas para servir `remoteEntry.js`.
- **BFF** (`bff/backend/`): gateway YARP + agregações; único backend publicado.
- **Domínios** (`business-units/`, `oauth/`): .NET 10, Clean Architecture, MongoDB próprio (database-per-service).

## 2. Regras de ouro (invariantes)

1. **O BFF é a única porta de entrada.** Nenhum MFE/browser/host fala direto com um microserviço
   interno. Serviços internos usam `expose` no compose, nunca `ports`.
2. **Nada compartilha assembly entre BFF e domínios.** O BFF fala HTTP + modelos próprios
   (`Models/Upstream` espelha o JSON). Domínios evoluem sem quebrar o BFF em compile time.
3. **Um banco por domínio.** Nenhum serviço lê o Mongo de outro; composição de dados é papel do BFF.
4. **Autorização fim-a-fim.** O token do usuário é validado no BFF (introspection) e **repassado**
   (token relay) aos domínios — o BFF não é um super-caller anônimo.
5. **Camadas por domínio:** `Domain ← Application ← Infrastructure ← API` (CQRS/MediatR); o BFF é
   Ports & Adapters em 3 camadas (sem Domain).
6. **MFE sem Router próprio** no `App.tsx` exposto — o Shell provê o Router (federation `shared`).

## 3. Mapa de portas e rede

| Serviço | Host | Interno | Publicado? |
|---------|------|---------|------------|
| shell | 3000 | 80 | ✅ |
| mfe-business-units | 3001 | 80 | ✅ (só p/ `remoteEntry.js`) |
| mfe-users | 3002 | 80 | ✅ (só p/ `remoteEntry.js`) |
| **bff-api** | **5002** | 8080 | ✅ **única API pública** |
| business-units-api | — | 8080 | ❌ `expose` |
| oauth-api | — | 8080 | ❌ `expose` |
| business-units-mongo | 27017 | 27017 | ✅ (dev only) |
| oauth-mongo | 27018 | 27017 | ✅ (dev only) |
| mongo-express | 8081/8082 | 8081 | ✅ (dev only) |
| **Novo MFE / novo domínio** | 3003+ / — | 80 / 8080 | MFE sim; API **não** (`expose`) |

Frontends apontam **só** para o BFF: `VITE_API_URL`, `VITE_OAUTH_API_URL`, `VITE_OAUTH_URL`
= `http://localhost:5002`.

## 4. Gateway (YARP) — especificação

Tabela de rotas montada **em memória** a partir de `DownstreamOptions` (`GatewayConfig.Build`):

| Rota | Cluster | Auth |
|------|---------|------|
| `/api/v1/users/{**}` | oauth-api | policy `ApiAccess` (token obrigatório) |
| `/api/v1/business-units/{**}` | business-units-api | `ApiAccess` |
| `/api/v1/businesses/{**}` | business-units-api | `ApiAccess` |
| `/connect/{**}`, `/account/{**}` | oauth-api | anônimo (o OIDC cuida da própria auth) |

- **Agregações** (`/api/v1/dashboard`, `/api/v1/businesses/{id}/overview`) são controllers MVC com
  rota de atributo — mais específicas que o catch-all, o MVC vence e não são proxeadas
  (garantido por teste de integração).
- `Program.cs`: `AddReverseProxy().LoadFromMemory(routes, clusters)` → `app.MapControllers()` →
  `app.MapReverseProxy()`.

### Correções críticas embutidas (não regredir)

1. **HTTP/1.1 forçado por cluster** — YARP proxeando como h2c perde o body de POST
   (`Sent 0 request content bytes...`):
   `HttpRequest = new ForwarderRequestConfig { Version = HttpVersion.Version11, VersionPolicy = RequestVersionExact }`.
2. **Buffering do body** — a validação OpenIddict lê o form body procurando `access_token` e
   consome o stream antes do YARP: `Request.EnableBuffering()` **antes** de `UseAuthentication`
   e `Request.Body.Position = 0` **antes** de `MapReverseProxy()`.
3. **Redirects host-relativos no OAuth server** — atrás do proxy o Host interno é
   `oauth-api:8080`; `OnRedirectToLogin/Logout/AccessDenied` reescrevem para `Uri.PathAndQuery`
   para o browser permanecer no origin público (`localhost:5002`).
4. **Issuer interno** — YARP reescreve o Host de `/connect/**` para `oauth-api:8080`, então
   `iss = http://oauth-api:8080`; o `OpenIddict:Issuer` do BFF **deve casar** esse valor.

## 5. Segurança

- **Tokens são JWE (criptografados)** — só o OpenIddict os inspeciona. Validação no BFF via
  `AddValidation().UseIntrospection()` com cliente confidencial semeado.
- **Audience do introspection (CRÍTICO):** o endpoint de introspection só devolve `active:true`
  se o **client que pergunta estiver nas audiences do token**. O cliente de introspection do BFF
  chama-se `resource_server` — o mesmo nome da audience emitida. Renomear um dos lados sem o
  outro derruba TODA a API com 401 silencioso (`{"active":false}`).
- **Token relay:** `BearerTokenPropagationHandler` repassa o `Authorization` do chamador aos
  domínios downstream.
- **Autorização por recurso:** `ICurrentUser.CanAccessBusiness(id)` = super admin **ou** permissão
  naquele business; handler lança `ForbiddenException` (→ 403) antes de vazar dados.
- **Modelo de permissões:** estruturado por escopo — `{ businessId, businessUnitId?, module,
  role, function? }` + flag `isSuperAdmin`. Módulos: `Business`, `BusinessUnit`, `Users`.
  Papéis: `BusinessAdmin`, `BusinessUnitAdmin`, `ModuleAdmin`, `Manager`, `Reader`, `Writer`.
- **Reinício do oauth-api invalida tokens existentes** (novas chaves de DataProtection) —
  usuários precisam relogar após deploy do OAuth server.
- Login **Google** requer forwarded headers com Host público (não configurado por padrão).

## 6. Padrão de agregação no BFF (fan-out)

```csharp
var a = _businessCatalog.CountBusinessesAsync(ct);
var b = _businessCatalog.CountBusinessUnitsAsync(ct);
var c = _userDirectory.CountUsersAsync(ct);
await Task.WhenAll(a, b, c);
```

Falha de um domínio → `UpstreamException` → **502** no `ExceptionMiddleware`.
404 downstream → `null` no client → `NotFoundException` (→ 404) no handler.

## 7. Restrições de licenciamento (não subir major sem licença)

| Lib | Versão fixada | Motivo |
|-----|---------------|--------|
| AutoMapper | **14.0.0** (MIT) | 15+ é comercial. Advisory NU1903 (DoS) aceito — patch só na versão comercial. |
| MediatR | **12.5.0** (Apache-2.0) | 13+ é comercial |
| FluentAssertions | **7.2.2** (Apache-2.0) | 8+ é comercial |

## 8. Implantação de frontend (restrição conhecida)

O nginx dos MFEs serve `/assets` (incluindo `remoteEntry.js`) com `expires 1y, immutable` —
**após um deploy o browser continua no bundle antigo** até hard-refresh. Qualquer especificação
que altere um MFE deve considerar: cache-control diferenciado para `remoteEntry.js` (no-cache)
ou versionamento da URL do remote no shell.

## 9. Como escrever uma especificação de arquitetura (template)

Ao especificar uma nova funcionalidade/domínio, produzir um documento com:

1. **Contexto e objetivo** — problema, usuários afetados, o que muda no sistema.
2. **Fronteiras de domínio** — a qual domínio pertence? É novo domínio (novo par backend+MFE) ou
   extensão de um existente? Quem é dono de cada dado (regra 3)?
3. **Contrato de API** — endpoints REST (`/api/v1/...`), payloads (camelCase), códigos de erro
   (400 domínio, 404 não encontrado, 422 validação, 502 upstream). PUT/PATCH: o `id` vem **da
   rota**; o body usa um record próprio **sem `Id`** (ver armadilha no SKILL.md raiz).
4. **Rota no gateway** — nova entrada em `DownstreamOptions` + rota YARP; agregações novas seguem
   a seção 6 e o passo a passo do SKILL.md raiz (Parte 6.2).
5. **Segurança** — quais permissões (`module`/`role`) protegem cada operação; a checagem fica no
   handler do BFF (agregações) e/ou no domínio.
6. **Dados** — coleções Mongo, índices (únicos?), class maps para coleções privadas.
7. **Frontend** — novo MFE (porta 3003+) ou tela em MFE existente; navegação no shell.
8. **Modos de falha** — o que acontece se o domínio X cair (502 esperado, degradação parcial?).
9. **Impacto em QA** — quais journeys e2e novos/afetados (ver skill `qa`).
10. **Decisões e alternativas** — registrar trade-offs no formato da seção 10.

## 10. Decisões registradas (mini-ADRs)

| Decisão | Motivo | Consequência |
|---------|--------|--------------|
| Introspection em vez de validação local de JWT | Tokens OpenIddict são JWE | BFF precisa ser cliente confidencial com nome = audience (`resource_server`) |
| YARP com rotas em memória | Reusar `DownstreamOptions` dos clients, sem config duplicada | Rotas nascem do mesmo lugar que os typed clients |
| HTTP/1.1 exato nos clusters | h2c perde body de POST | Sem HTTP/2 até o proxy suportar de ponta a ponta |
| Redirects host-relativos no cookie handler | Host interno não é alcançável pelo browser | Google SSO exige configuração extra de forwarded headers |
| `expose` (sem `ports`) nos serviços internos | Garantir a regra da porta única na prática | Debug direto do domínio requer `docker exec` ou porta temporária |
| Versões livres de AutoMapper/MediatR/FluentAssertions | Evitar licença comercial | Não acompanhar os majors mais novos |
