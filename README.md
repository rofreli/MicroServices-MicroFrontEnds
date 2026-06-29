# Platform — Business Suite

Plataforma de gestão empresarial baseada em micro-frontends, com autenticação OAuth 2.0 e APIs RESTful em .NET 8.

---

## Estrutura do repositório

```
repos/
├── docker-compose.yml          ← Orquestrador raiz (roda tudo)
├── shell/                      ← Frontend host (MFE shell, porta 3000)
├── business-units/
│   ├── backend/                ← API de Unidades de Negócio (.NET 8, porta 5000)
│   └── mfe-business-units/     ← MFE de Unidades de Negócio (porta 3001)
└── oauth/
    ├── backend/                ← API OAuth 2.0 / Usuários (.NET 8, porta 5001)
    └── mfe-users/              ← MFE de Gestão de Usuários (porta 3002)
```

---

## Pré-requisitos

| Ferramenta | Versão mínima |
|------------|---------------|
| Docker     | 24+           |
| Docker Compose | v2.20+    |

---

## Início rápido

```bash
# Na raiz do repositório
cd repos/

# (Opcional) Configurar Google SSO — ver seção abaixo
export GOOGLE_CLIENT_ID=seu_client_id
export GOOGLE_CLIENT_SECRET=seu_client_secret

# Build e start de todos os serviços
docker compose up --build -d

# Acompanhar logs
docker compose logs -f

# Parar tudo
docker compose down
```

Após o start, aguarde ~30 s para os bancos de dados subirem e as APIs inicializarem.
Acesse: **http://localhost:3000**

---

## Serviços provisionados

### Frontends

| Serviço | Container | Porta | Descrição |
|---------|-----------|-------|-----------|
| Shell (host) | `platform-shell` | `3000` | App React principal — carrega os MFEs via Module Federation |
| MFE Business Units | `bu-mfe-business-units` | `3001` | Módulo de gestão de unidades de negócio |
| MFE Users | `oauth-mfe-users` | `3002` | Módulo de gestão de usuários e roles |

### APIs (.NET 8)

| Serviço | Container | Porta | Swagger |
|---------|-----------|-------|---------|
| Business Units API | `business-units-api` | `5000` | http://localhost:5000/swagger |
| OAuth API | `oauth-api` | `5001` | http://localhost:5001/swagger |

### Bancos de dados (MongoDB 7)

| Serviço | Container | Porta (host) | Database |
|---------|-----------|--------------|----------|
| MongoDB — Business Units | `business-units-mongo` | `27017` | `BusinessUnitsDb` |
| MongoDB — OAuth | `oauth-mongo` | `27018` | `OAuthDb` |

### Admin MongoDB (Mongo Express)

| Interface | Container | URL | Credenciais |
|-----------|-----------|-----|-------------|
| Business Units | `business-units-mongo-express` | http://localhost:8081 | admin / admin123 |
| OAuth | `oauth-mongo-express` | http://localhost:8082 | admin / admin123 |

---

## Rede Docker

Todos os serviços compartilham a rede `platform-net` (bridge). Os containers se comunicam pelo nome do container (ex.: `mongodb://business-units-mongo:27017`). As portas expostas acima são apenas para acesso do host.

---

## Arquitetura de autenticação

```
Usuário
  │
  ├─► Shell (3000) ──► /connect/authorize ──► Login.cshtml (OAuth API)
  │                                                │
  │                            ┌──────────────────┤
  │                            │  Local (email/senha)
  │                            │  Google SSO
  │                            └──────────────────┐
  │                                               │
  └◄── Authorization Code (PKCE) ◄────────────────┘
         │
         ▼
  POST /connect/token ──► Access Token JWT
         │
         ▼
  Requisições autenticadas para as APIs (Bearer token)
```

O shell implementa o fluxo **Authorization Code + PKCE** manualmente (sem biblioteca OIDC). O token é armazenado no `localStorage` e injetado pelo `mfe-users` via `axios interceptor`.

---

## Configurar Google SSO

1. Acesse [Google Cloud Console](https://console.cloud.google.com/) → APIs & Services → Credentials
2. Crie um **OAuth 2.0 Client ID** do tipo *Web application*
3. Em *Authorized redirect URIs*, adicione: `http://localhost:5001/account/callback`
4. Exporte as variáveis antes do `docker compose up`:

```bash
export GOOGLE_CLIENT_ID="xxxx.apps.googleusercontent.com"
export GOOGLE_CLIENT_SECRET="GOCSPX-xxxx"
docker compose up --build -d
```

Ou crie um arquivo `.env` na raiz:

```env
GOOGLE_CLIENT_ID=xxxx.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=GOCSPX-xxxx
```

---

## Comandos úteis

```bash
# Ver status de todos os containers
docker compose ps

# Logs de um serviço específico
docker compose logs -f oauth-api
docker compose logs -f shell

# Reconstruir apenas um serviço
docker compose up --build -d mfe-users

# Parar sem remover volumes (preserva dados do MongoDB)
docker compose stop

# Parar E remover volumes (reset completo dos bancos)
docker compose down -v
```

---

## Desenvolvimento local (sem Docker)

Para desenvolver com hot-reload, rode cada serviço individualmente:

```bash
# Terminal 1 — API Business Units
cd business-units/backend
dotnet run --project src/BusinessUnits.API

# Terminal 2 — API OAuth
cd oauth/backend
dotnet run --project src/OAuth.API

# Terminal 3 — Shell
cd shell && npm install && npm run dev  # http://localhost:3000

# Terminal 4 — MFE Business Units
cd business-units/mfe-business-units && npm install && npm run dev  # http://localhost:3001

# Terminal 5 — MFE Users
cd oauth/mfe-users && npm install && npm run dev  # http://localhost:3002
```

Os bancos de dados ainda podem ser rodados via Docker:

```bash
docker compose up -d business-units-mongo oauth-mongo
```

---

## Variáveis de ambiente relevantes

| Variável | Padrão | Descrição |
|----------|--------|-----------|
| `GOOGLE_CLIENT_ID` | `CONFIGURE_ME` | Client ID do Google OAuth |
| `GOOGLE_CLIENT_SECRET` | `CONFIGURE_ME` | Client Secret do Google OAuth |

As demais variáveis (conexões MongoDB, CORS, URLs dos MFEs) estão configuradas diretamente no `docker-compose.yml` com valores adequados para desenvolvimento local.
