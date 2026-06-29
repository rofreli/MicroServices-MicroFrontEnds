# Business Units

API e micro-frontend para gerenciamento de unidades de negócio.  
O shell (host app) fica na raiz do repositório pois é compartilhado entre múltiplos MFEs.

## Estrutura

```
business-units/
├── backend/                         # .NET 8 — Clean Architecture + CQRS
│   ├── BusinessUnits.sln
│   └── src/
│       ├── BusinessUnits.Domain/        # Entidades, VOs, Eventos, Interfaces
│       ├── BusinessUnits.Application/   # Commands, Queries, DTOs, Validators
│       ├── BusinessUnits.Infrastructure/# MongoDB repositories
│       └── BusinessUnits.API/           # Controllers, Middleware, Program.cs
├── mfe-business-units/              # React + Vite + Module Federation (porta 3001)
└── docker-compose.yml               # API + MongoDB + MFE
```

## Início rápido

```bash
# Sobe API + MongoDB + MFE
docker compose up --build
```

| Serviço | URL |
|---------|-----|
| MFE (standalone) | http://localhost:3001 |
| API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |
| Mongo Express | http://localhost:8081 |

Para rodar o shell (host app), vá em `../shell/`.

## API Endpoints

```
GET    /api/v1/business-units
GET    /api/v1/business-units/{id}
GET    /api/v1/business-units/cnpj/{cnpj}
POST   /api/v1/business-units
PUT    /api/v1/business-units/{id}
DELETE /api/v1/business-units/{id}
```

## Dev local

```bash
# MFE
cd mfe-business-units && npm install && npm run dev

# API (requer .NET 8 SDK)
cd backend && dotnet run --project src/BusinessUnits.API
```
