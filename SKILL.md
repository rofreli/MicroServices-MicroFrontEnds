# Skill: Criar Novo Microserviço

Guia completo para adicionar um novo domínio ao monorepo `/home/rodrigo/pessoal/repos/`.
Cada domínio tem um **backend .NET 8** e um **MFE React**. Exemplos de referência: `business-units/` e `oauth/`.

---

## Estrutura do Monorepo

```
/repos
├── docker-compose.yml          ← orquestra todos os serviços
├── shell/                      ← Shell principal (host MFE)
├── <domain>/
│   ├── backend/                ← API .NET 8
│   └── mfe-<domain>/           ← Micro-frontend React
└── oauth/
    ├── backend/                ← OAuth server (não replicar)
    └── mfe-users/              ← MFE de usuários
```

---

## Parte 1 — Backend .NET 8

### 1.1 Estrutura de projetos (Clean Architecture)

```
<Domain>/backend/src/
├── <Domain>.Domain/            ← Sdk="Microsoft.NET.Sdk"
├── <Domain>.Application/       ← Sdk="Microsoft.NET.Sdk"
├── <Domain>.Infrastructure/    ← Sdk="Microsoft.NET.Sdk"
└── <Domain>.API/               ← Sdk="Microsoft.NET.Sdk.Web"
```

Criar solução:
```bash
cd <domain>/backend
dotnet new sln -n <Domain>
dotnet new classlib -n <Domain>.Domain       -o src/<Domain>.Domain       --framework net8.0
dotnet new classlib -n <Domain>.Application  -o src/<Domain>.Application  --framework net8.0
dotnet new classlib -n <Domain>.Infrastructure -o src/<Domain>.Infrastructure --framework net8.0
dotnet new webapi   -n <Domain>.API          -o src/<Domain>.API          --framework net8.0
dotnet sln add src/**/*.csproj
```

### 1.2 Referências entre projetos

```
API        → Application + Infrastructure
Infrastructure → Domain + Application
Application  → Domain
Domain       → (nenhuma)
```

### 1.3 Pacotes NuGet por camada

**`<Domain>.Domain.csproj`** — sem pacotes externos (apenas regras de negócio puras)

**`<Domain>.Application.csproj`**
```xml
<PackageReference Include="MediatR" Version="12.2.0" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
<PackageReference Include="FluentValidation" Version="11.9.2" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.2" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
```

**`<Domain>.Infrastructure.csproj`**
```xml
<PackageReference Include="MongoDB.Driver" Version="2.27.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Options" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Options.ConfigurationExtensions" Version="8.0.0" />
```

**`<Domain>.API.csproj`**
```xml
<PackageReference Include="MediatR" Version="12.2.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
```

> **ATENÇÃO — versões NuGet críticas:**
> - `Microsoft.Extensions.Options.ConfigurationExtensions` **não tem versão 8.0.1** — usar `8.0.0`
> - `Microsoft.Extensions.Configuration.Abstractions` **não tem versão 8.0.1** — usar `8.0.0`
> - `Microsoft.Extensions.Options` e `DependencyInjection.Abstractions` **têm** versão `8.0.1`
> - Se um pacote transitivo exigir versão maior, **nunca misturar major** (9.x com 8.x causa NU1605)

### 1.4 Domain Layer

**Entidade base:**
```csharp
// src/<Domain>.Domain/Entities/<Entity>.cs
namespace <Domain>.Domain.Entities;

public class <Entity>
{
    public string Id { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private <Entity>() { }  // ← construtor privado para MongoDB

    public static <Entity> Create(/* params */)
    {
        return new <Entity>
        {
            Id = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void Update(/* params */)
    {
        // mutações...
        UpdatedAt = DateTime.UtcNow;
    }
}
```

**Coleções privadas** (padrão invariante):
```csharp
private readonly List<Item> _items = new();
public IReadOnlyList<Item> Items => _items.AsReadOnly();
```

**Exceções de domínio:**
```csharp
// src/<Domain>.Domain/Exceptions/DomainException.cs
namespace <Domain>.Domain.Exceptions;
public class DomainException : Exception { public DomainException(string msg) : base(msg) { } }
public class NotFoundException : Exception { public NotFoundException(string entity, string id) : base($"{entity} '{id}' not found.") { } }
public class ConflictException : Exception { public ConflictException(string msg) : base(msg) { } }
```

**Interface de repositório:**
```csharp
// src/<Domain>.Domain/Interfaces/I<Entity>Repository.cs
namespace <Domain>.Domain.Interfaces;

public interface I<Entity>Repository
{
    Task<Entity?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<Entity>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<long> CountAsync(CancellationToken ct = default);
    Task AddAsync(Entity entity, CancellationToken ct = default);
    Task UpdateAsync(Entity entity, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
```

### 1.5 Application Layer (CQRS)

**DependencyInjection.cs:**
```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace <Domain>.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
```

**ValidationBehavior.cs** (copiar de `business-units/backend/src/BusinessUnits.Application/Behaviors/ValidationBehavior.cs`):
```csharp
namespace <Domain>.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
            throw new FluentValidation.ValidationException(failures);

        return await next();
    }
}
```

**Command (record):**
```csharp
// src/<Domain>.Application/Commands/Create<Entity>/Create<Entity>Command.cs
using MediatR;

namespace <Domain>.Application.Commands.Create<Entity>;

public record Create<Entity>Command(
    string Field1,
    string Field2
) : IRequest<<Entity>Dto>;
```

**CommandHandler:**
```csharp
// src/<Domain>.Application/Commands/Create<Entity>/Create<Entity>CommandHandler.cs
using AutoMapper;
using MediatR;

namespace <Domain>.Application.Commands.Create<Entity>;

public class Create<Entity>CommandHandler : IRequestHandler<Create<Entity>Command, <Entity>Dto>
{
    private readonly I<Entity>Repository _repository;
    private readonly IMapper _mapper;

    public Create<Entity>CommandHandler(I<Entity>Repository repository, IMapper mapper)
        => (_repository, _mapper) = (repository, mapper);

    public async Task<<Entity>Dto> Handle(Create<Entity>Command request, CancellationToken ct)
    {
        var entity = <Entity>.Create(request.Field1, request.Field2);
        await _repository.AddAsync(entity, ct);
        return _mapper.Map<<Entity>Dto>(entity);
    }
}
```

**Query (record):**
```csharp
public record GetAll<Entity>Query(int Page, int PageSize) : IRequest<PaginatedResult<<Entity>Dto>>;
```

**PaginatedResult.cs:**
```csharp
namespace <Domain>.Application.Common;

public record PaginatedResult<T>(IReadOnlyList<T> Items, long Total, int Page, int PageSize);
```

**MappingProfile:**
```csharp
using AutoMapper;

namespace <Domain>.Application.Mappings;

public class <Entity>MappingProfile : Profile
{
    public <Entity>MappingProfile()
    {
        CreateMap<<Entity>, <Entity>Dto>();
    }
}
```

### 1.6 Infrastructure Layer

**MongoDbSettings.cs:**
```csharp
namespace <Domain>.Infrastructure.Persistence;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "<Domain>Db";
    public string <Entity>sCollection { get; set; } = "<entity>s";
}
```

**MongoDbContext.cs:**
```csharp
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace <Domain>.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;

    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        MongoDbClassMaps.Register();
        _settings = options.Value;
        var client = new MongoClient(_settings.ConnectionString);
        _database = client.GetDatabase(_settings.DatabaseName);
        EnsureIndexes();
    }

    public IMongoDatabase Database => _database;
    public IMongoCollection<<Entity>> <Entity>s =>
        _database.GetCollection<<Entity>>(_settings.<Entity>sCollection);

    private void EnsureIndexes()
    {
        // Adicionar índices únicos aqui se necessário
    }
}
```

**MongoDbClassMaps.cs** — necessário para entidades com coleções privadas:
```csharp
using MongoDB.Bson.Serialization;

namespace <Domain>.Infrastructure.Configurations;

public static class MongoDbClassMaps
{
    private static bool _registered;

    public static void Register()
    {
        if (_registered) return;
        _registered = true;

        BsonClassMap.RegisterClassMap<<Entity>>(map =>
        {
            map.AutoMap();
            // Para coleções privadas: usar MapField, não MapMember
            map.MapField("_items").SetElementName("items");
        });
    }
}
```

> **CRÍTICO:** Para propriedades `IReadOnlyList<T>` com backing field privado `_field`,
> usar `map.MapField("_field")` — `map.MapMember(x => x.Prop)` falha em desserialização.

**Repository:**
```csharp
using MongoDB.Driver;

namespace <Domain>.Infrastructure.Persistence.Repositories;

public class <Entity>Repository : I<Entity>Repository
{
    private readonly IMongoCollection<<Entity>> _collection;

    public <Entity>Repository(MongoDbContext context) => _collection = context.<Entity>s;

    public async Task<<Entity>?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<<Entity>>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var result = await _collection
            .Find(Builders<<Entity>>.Filter.Empty)
            .SortByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);
        return result.AsReadOnly();
    }

    public async Task<long> CountAsync(CancellationToken ct = default)
        => await _collection.CountDocumentsAsync(Builders<<Entity>>.Filter.Empty, cancellationToken: ct);

    public async Task AddAsync(<Entity> entity, CancellationToken ct = default)
        => await _collection.InsertOneAsync(entity, cancellationToken: ct);

    public async Task UpdateAsync(<Entity> entity, CancellationToken ct = default)
        => await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: ct);

    public async Task DeleteAsync(string id, CancellationToken ct = default)
        => await _collection.DeleteOneAsync(x => x.Id == id, ct);
}
```

> **ATENÇÃO MongoDB LINQ:** Tipos value object customizados (ex: `Email`, `Cnpj`) não são traduzíveis
> pelo LINQ do MongoDB Driver. Para filtros por esses campos usar `Builders<T>.Filter.Eq("FieldName", value)`.

**DependencyInjection.cs:**
```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace <Domain>.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<I<Entity>Repository, <Entity>Repository>();
        return services;
    }
}
```

### 1.7 API Layer

**Program.cs:**
```csharp
using <Domain>.API.Middleware;
using <Domain>.Application;
using <Domain>.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "<Domain> API", Version = "v1" }));

builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
            builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
            ?? new[] { "http://localhost:3000" })
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "<Domain> API v1"));
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();
```

**ExceptionMiddleware.cs** — copiar de `business-units` e ajustar namespace:
```csharp
// Mapeia: ValidationException → 422, NotFoundException → 404,
//         DomainException → 400, outros → 500
// Resposta JSON: { status, message, errors[] }
```

**Controller:**
```csharp
[ApiController]
[Route("api/v1/<entities>")]
[Produces("application/json")]
public class <Entity>sController : ControllerBase
{
    private readonly IMediator _mediator;
    public <Entity>sController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetAll<Entity>sQuery(page, pageSize), ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
        => Ok(await _mediator.Send(new Get<Entity>ByIdQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Create<Entity>Command command, CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Update<Entity>Command command, CancellationToken ct = default)
        => Ok(await _mediator.Send(command with { Id = id }, ct));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct = default)
    {
        await _mediator.Send(new Delete<Entity>Command(id), ct);
        return NoContent();
    }
}
```

**appsettings.json:**
```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "<Domain>Db",
    "<Entity>sCollection": "<entity>s"
  },
  "Cors": {
    "Origins": ["http://localhost:3000", "http://localhost:<mfe-port>"]
  },
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }
  },
  "AllowedHosts": "*"
}
```

### 1.8 Dockerfile do Backend

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/ .
RUN dotnet restore <Domain>.API/<Domain>.API.csproj
RUN dotnet publish <Domain>.API/<Domain>.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "<Domain>.API.dll"]
```

---

## Parte 2 — MFE React (Vite + Module Federation)

### 2.1 Estrutura de arquivos

```
<domain>/mfe-<domain>/
├── Dockerfile
├── nginx.conf
├── package.json
├── tsconfig.json
├── tsconfig.node.json
├── vite.config.ts
└── src/
    ├── App.tsx         ← componente exportado (SEM Router)
    ├── main.tsx        ← entry point standalone (COM Router)
    ├── routes.tsx      ← apenas <Routes>, sem <Router>
    ├── vite-env.d.ts
    ├── api/
    │   ├── client.ts   ← axios com baseURL via env var
    │   └── <entity>s.ts
    ├── components/
    ├── hooks/
    ├── pages/
    │   ├── <Entity>sList.tsx
    │   ├── <Entity>Create.tsx
    │   ├── <Entity>Detail.tsx
    │   └── <Entity>Edit.tsx
    └── types/
        └── <entity>.ts
```

### 2.2 package.json

```json
{
  "name": "@<domain>/mfe-<domain>",
  "version": "1.0.0",
  "private": true,
  "scripts": {
    "dev": "vite --port <PORT>",
    "build": "tsc && vite build",
    "preview": "vite preview --port <PORT>"
  },
  "dependencies": {
    "react": "^18.3.1",
    "react-dom": "^18.3.1",
    "react-router-dom": "^6.28.0",
    "@tanstack/react-query": "^5.62.7",
    "react-hook-form": "^7.54.1",
    "axios": "^1.7.9"
  },
  "devDependencies": {
    "@types/react": "^18.3.14",
    "@types/react-dom": "^18.3.5",
    "@vitejs/plugin-react": "^4.3.4",
    "@originjs/vite-plugin-federation": "^1.3.6",
    "typescript": "^5.7.2",
    "vite": "^5.4.11",
    "tailwindcss": "^3.4.17",
    "autoprefixer": "^10.4.20",
    "postcss": "^8.4.49"
  }
}
```

### 2.3 vite.config.ts

```ts
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import federation from '@originjs/vite-plugin-federation'

export default defineConfig({
  plugins: [
    react(),
    federation({
      name: 'mfe<Domain>',              // camelCase, único no monorepo
      filename: 'remoteEntry.js',
      exposes: {
        './App': './src/App',
        './routes': './src/routes',
      },
      shared: ['react', 'react-dom', 'react-router-dom'],
    }),
  ],
  build: { target: 'esnext', minify: false, cssCodeSplit: false },
  server: { port: <PORT>, cors: true },
  preview: { port: <PORT>, cors: true },
})
```

### 2.4 tsconfig.json

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "skipLibCheck": true,
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "jsx": "react-jsx",
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true
  },
  "include": ["src"],
  "references": [{ "path": "./tsconfig.node.json" }]
}
```

### 2.5 App.tsx — SEM Router

```tsx
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { <Domain>Routes } from './routes'
import './index.css'

const queryClient = new QueryClient({
  defaultOptions: { queries: { staleTime: 1000 * 60 * 5, retry: 1 } },
})

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <div className="mfe-<domain>">
        <<Domain>Routes />
      </div>
    </QueryClientProvider>
  )
}
```

> **CRÍTICO:** `App.tsx` **não deve ter `<BrowserRouter>`, `<MemoryRouter>` ou qualquer `<Router>`**.
> O Shell já provê um `<BrowserRouter>` e `react-router-dom` é `shared` — qualquer Router
> aninhado causa `invariant Error` no React Router v6.

### 2.6 main.tsx — COM Router (modo standalone/dev)

```tsx
import React from 'react'
import ReactDOM from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { <Domain>Routes } from './routes'
import './index.css'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <QueryClientProvider client={new QueryClient()}>
      <BrowserRouter basename="/<domain>">
        <<Domain>Routes />
      </BrowserRouter>
    </QueryClientProvider>
  </React.StrictMode>,
)
```

### 2.7 routes.tsx — apenas Routes

```tsx
import { Route, Routes } from 'react-router-dom'
import { <Entity>sList } from './pages/<Entity>sList'
import { <Entity>Create } from './pages/<Entity>Create'
import { <Entity>Detail } from './pages/<Entity>Detail'
import { <Entity>Edit } from './pages/<Entity>Edit'

export function <Domain>Routes() {
  return (
    <Routes>
      <Route index element={<<Entity>sList />} />
      <Route path="new" element={<<Entity>Create />} />
      <Route path=":id" element={<<Entity>Detail />} />
      <Route path=":id/edit" element={<<Entity>Edit />} />
    </Routes>
  )
}
```

### 2.8 api/client.ts

```ts
import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:<API_PORT>'

export const apiClient = axios.create({
  baseURL: `${API_BASE_URL}/api/v1`,
  headers: { 'Content-Type': 'application/json' },
})

apiClient.interceptors.response.use(
  (res) => res,
  (error) => {
    const message = error.response?.data?.message ?? error.message ?? 'Erro desconhecido'
    return Promise.reject(new Error(message))
  },
)
```

### 2.9 nginx.conf

```nginx
server {
    listen 80;
    root /usr/share/nginx/html;
    index index.html;

    add_header Access-Control-Allow-Origin *;
    add_header Access-Control-Allow-Methods "GET, OPTIONS";
    add_header Access-Control-Allow-Headers "*";

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /assets {
        expires 1y;
        add_header Cache-Control "public, immutable";
        add_header Access-Control-Allow-Origin *;
        add_header Access-Control-Allow-Methods "GET, OPTIONS";
        add_header Access-Control-Allow-Headers "*";
    }
}
```

> **CRÍTICO:** Em Nginx, `add_header` em um bloco `location` filho **substitui** todos os headers
> do bloco pai — não herda. O bloco `/assets` **deve repetir** os headers CORS,
> caso contrário o `remoteEntry.js` é servido sem CORS → `ERR_FAILED 200`.

### 2.10 Dockerfile do MFE

```dockerfile
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

---

## Parte 3 — Integrar ao Shell

Ao adicionar um novo MFE, editar **5 arquivos** no `shell/`:

### 3.1 `shell/vite.config.ts` — adicionar remote

```ts
const MFE_<DOMAIN>_URL = process.env.VITE_MFE_<DOMAIN>_URL ?? 'http://localhost:<PORT>'

// dentro de federation({ remotes: { ... } }):
mfe<Domain>: {
  external: `${MFE_<DOMAIN>_URL}/assets/remoteEntry.js`,
  from: 'vite',
  externalType: 'url',
},
```

### 3.2 `shell/src/types/remotes.d.ts` — declarar módulo

```ts
declare module 'mfe<Domain>/App' {
  import type { ComponentType } from 'react'
  const App: ComponentType
  export default App
}

declare module 'mfe<Domain>/routes' {
  import type { ComponentType } from 'react'
  export const <Domain>Routes: ComponentType
}
```

### 3.3 `shell/src/App.tsx` — adicionar rota lazy

```tsx
function <Domain>MFE() {
  return <MicroFrontendLoader loader={() => import('mfe<Domain>/App')} />
}

// dentro de <Routes>:
<Route path="/<domain>/*" element={<<Domain>MFE />} />
```

### 3.4 `shell/src/components/Sidebar.tsx` — adicionar item de navegação

```tsx
// Adicionar ícone SVG inline
function Icon<Domain>() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.75"
         strokeLinecap="round" strokeLinejoin="round" className="w-5 h-5 shrink-0">
      {/* caminho SVG adequado ao domínio */}
    </svg>
  )
}

// Na array navItems:
{ label: '<Label>', path: '/<domain>', icon: <Icon<Domain> /> },
```

### 3.5 `docker-compose.yml` — adicionar build arg + serviço

```yaml
# Em shell.build.args:
VITE_MFE_<DOMAIN>_URL: http://localhost:<PORT>

# Novo serviço MFE:
mfe-<domain>:
  build:
    context: ./<domain>/mfe-<domain>
    dockerfile: Dockerfile
  container_name: <domain>-mfe-<domain>
  ports:
    - "<PORT>:80"
  networks:
    - platform-net
  restart: unless-stopped

# Nova API backend:
<domain>-api:
  build:
    context: ./<domain>/backend
    dockerfile: src/<Domain>.API/Dockerfile
  container_name: <domain>-api
  ports:
    - "<API_PORT>:8080"
  environment:
    ASPNETCORE_ENVIRONMENT: Production
    MongoDb__ConnectionString: mongodb://<domain>-mongo:27017
    MongoDb__DatabaseName: <Domain>Db
    Cors__Origins__0: http://localhost:3000
    Cors__Origins__1: http://localhost:<PORT>
  depends_on:
    <domain>-mongo:
      condition: service_healthy
  networks:
    - platform-net
  restart: unless-stopped

<domain>-mongo:
  image: mongo:7
  container_name: <domain>-mongo
  ports:
    - "<MONGO_PORT>:27017"
  volumes:
    - <domain>-mongo-data:/data/db
  healthcheck:
    test: ["CMD", "mongosh", "--eval", "db.adminCommand('ping')"]
    interval: 10s
    timeout: 5s
    retries: 5
  networks:
    - platform-net
  restart: unless-stopped

# Em volumes:
<domain>-mongo-data:
```

---

## Parte 4 — Checklist de portas

| Serviço | Porta externa | Porta interna |
|---------|--------------|---------------|
| Shell | 3000 | 80 |
| mfe-business-units | 3001 | 80 |
| mfe-users | 3002 | 80 |
| **Novo MFE** | **3003+** | **80** |
| business-units-api | 5000 | 8080 |
| oauth-api | 5001 | 8080 |
| **Nova API** | **5002+** | **8080** |
| business-units-mongo | 27017 | 27017 |
| oauth-mongo | 27018 | 27017 |
| **Novo Mongo** | **27019+** | **27017** |

---

## Parte 5 — Armadilhas conhecidas

| Situação | Sintoma | Solução |
|----------|---------|---------|
| Router aninhado no MFE | `invariant Error` no Router | Remover `<MemoryRouter>` do `App.tsx`; manter só no `main.tsx` |
| CORS no `/assets` do Nginx | `ERR_FAILED 200 (OK)` no `remoteEntry.js` | Repetir `add_header CORS` dentro do `location /assets` |
| Pacote sem versão `8.0.1` | NU1603 / NU1605 cascadeando para `9.0.0` | Verificar NuGet; `ConfigurationExtensions` e `Configuration.Abstractions` ficam em `8.0.0` |
| Controller herda `Controller` e tem classe `User` | CS1061 em `User.MétodoEstático()` | Alias: `using DomainUser = Namespace.User;` |
| `Request.PathAndQuery` em ASP.NET Core | CS1061 | Usar `$"{Request.Path}{Request.QueryString}"` |
| `IAsyncEnumerable.ToListAsync()` | CS1061 | Usar `await foreach` + `List<T>` manual |
| MongoDB LINQ com value objects | Exceção em runtime | Usar `Builders<T>.Filter.Eq("Field", value)` |
| `MapMember(x => x.ReadOnlyProp)` no BsonClassMap | Falha na desserialização silenciosa | Usar `MapField("_backingField")` |
| `GetOpenIddictServerRequest()` não encontrado | CS1061 | `using Microsoft.AspNetCore;` (não `OpenIddict.Server.AspNetCore`) |
