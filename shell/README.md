# Shell — Platform Host

Aplicação host (micro-frontend shell) responsável por compor todos os MFEs da plataforma.  
Fica na raiz do repositório pois é compartilhada entre múltiplos domínios/APIs.

## Estrutura

```
shell/
├── src/
│   ├── App.tsx                  # Roteamento + lazy-load dos MFEs
│   ├── main.tsx
│   ├── components/
│   │   ├── Layout.tsx           # Sidebar + Header + <Outlet>
│   │   ├── Sidebar.tsx          # Navegação entre módulos
│   │   ├── Header.tsx
│   │   └── MicroFrontendLoader.tsx  # Wrapper React.lazy para MFEs
│   ├── pages/
│   │   └── Dashboard.tsx
│   └── types/
│       └── remotes.d.ts         # Declarações de tipos dos MFEs remotos
├── vite.config.ts               # Module Federation — lista de remotes
└── docker-compose.yml           # Sobe apenas o shell (conecta à rede bu-net)
```

## Adicionar um novo MFE

1. Registre o remote em [vite.config.ts](vite.config.ts):
```ts
remotes: {
  mfeBusinessUnits: { external: 'http://localhost:3001/assets/remoteEntry.js', ... },
  mfeOutroModulo:   { external: 'http://localhost:3002/assets/remoteEntry.js', ... },
}
```

2. Declare o tipo em [src/types/remotes.d.ts](src/types/remotes.d.ts):
```ts
declare module 'mfeOutroModulo/App' { ... }
```

3. Adicione a rota em [src/App.tsx](src/App.tsx):
```tsx
<Route path="/outro-modulo/*" element={
  <MicroFrontendLoader loader={() => import('mfeOutroModulo/App')} />
} />
```

4. Adicione o item na sidebar em [src/components/Sidebar.tsx](src/components/Sidebar.tsx).

## Dev local

```bash
npm install && npm run dev
# → http://localhost:3000
# (os MFEs precisam estar rodando nas respectivas portas)
```

## Docker

```bash
# Garanta que a rede bu-net existe (subindo business-units primeiro)
cd ../business-units && docker compose up -d

# Depois sobe o shell
docker compose up --build
```
