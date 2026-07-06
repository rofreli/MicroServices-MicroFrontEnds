# Dossiê de Produto: Platform Business Suite

**Última verificação no produto vivo:** 2026-07-06 (Playwright MCP, super admin)
**Stack local:** `docker compose up -d` na raiz · UI em `http://localhost:3000` · API única em `http://localhost:5002` (BFF)
**Credenciais seed:** `rodrigofreitas218@gmail.com` / `Admin@1234` (super admin)

---

## 1. O que o produto é

Suíte B2B de gestão com dois módulos: **Empresas** (empresas + unidades de negócio/filiais, com
CNPJ) e **Usuários** (contas, permissões por escopo, ativação). Shell único com dashboard de
contadores. Login por e-mail/senha (OAuth próprio) com botão "Entrar com Google".

## 2. Mapa de telas e ações (verificado)

| Tela | Rota | Ações disponíveis |
|------|------|-------------------|
| Login (OAuth) | `/account/login` | entrar; entrar com Google |
| Dashboard | `/` | ver contadores (Empresas, Unidades, Usuários); atalhos p/ módulos |
| Empresas (lista) | `/business-units` | criar; abrir detalhe (click na linha); editar; excluir (confirm nativo) |
| Nova/Editar Empresa | `/business-units/new`, `/:id/edit` | razão social, nome fantasia, CNPJ (só na criação) |
| Detalhe da Empresa | `/business-units/:id` | ver dados; editar; **gerir unidades inline**: criar (razão/fantasia/CNPJ) e excluir |
| Usuários (lista) | `/users` | criar; abrir detalhe; editar; inativar/ativar |
| Novo/Editar Usuário | `/users/new`, `/:id/edit` | nome, sobrenome, e-mail+senha (só na criação) |
| Detalhe do Usuário | `/users/:id` | status, tipo (Super Admin/Padrão), SSO; **permissões**: adicionar (ID da empresa + módulo + papel) e remover; inativar/ativar |

## 3. Regras de negócio confirmadas (com evidência)

| # | Regra | Evidência |
|---|-------|-----------|
| RN1 | CNPJ é validado com dígitos verificadores no servidor; inválido → erro | UI 06/07: "CNPJ '...' is invalid." |
| RN2 | CNPJ é único (empresa e unidade); **imutável** após criação (edição não expõe o campo) | índices únicos; `BusinessEdit.tsx` |
| RN3 | Unidade de negócio **exige** empresa-mãe (`businessId`) | `BusinessUnit.Create` lança sem businessId |
| RN4 | Excluir empresa **com unidades é bloqueado** — remover unidades antes | `DeleteBusinessCommandHandler` (DomainException) |
| RN5 | E-mail de usuário é único | UI 06/07: "User with email '...' already exists." |
| RN6 | Senha: mín. 8 + ≥1 maiúscula + ≥1 número | `CreateUserCommandValidator` |
| RN7 | Permissão = escopo `{empresa, [unidade], módulo, papel, [função]}`; adicionar no mesmo escopo **substitui** | `User.AddPermission` |
| RN8 | Super admin tem acesso total (badge própria; permissões viram informativas) | UI + `ICurrentUser` |
| RN9 | Inativar/ativar usuário são idempotentes-protegidos (repetir lança erro de domínio) | `User.Deactivate/Activate` |
| RN10 | Módulos: `Business`, `BusinessUnit`, `Users` · Papéis: `BusinessAdmin`, `BusinessUnitAdmin`, `ModuleAdmin`, `Manager`, `Reader`, `Writer` · Funções: `InviteUser` | `types/user.ts` |

## 4. Lacunas e dívidas conhecidas (achados de sondagem)

**Funcionalidade ausente (visível ao usuário)**
1. **Não existe exclusão de usuário** — só inativar. Contas de teste acumulam (7 usuários no
   ambiente, 6 residuais). Atenção LGPD/direito ao apagamento.
2. **Empresa/Unidade não têm inativar/ativar** — lista mostra badge "Ativa", mas não há ação;
   inconsistente com Usuários.
3. **Unidade não tem edição na UI** (backend tem PUT) — só criar/excluir no detalhe da empresa.
4. **Recursos fantasmas**: backend suporta **endereço e contatos** de unidade
   (`Address`, `Contacts` no `CreateBusinessUnitBody`) e a função **`InviteUser`** — nada disso
   existe na UI. Busca por CNPJ (`GET /business-units/cnpj/{cnpj}`) idem.
5. **Listas sem busca, filtro ou ordenação** (empresas e usuários). Paginação só aparece com >20.
6. Sem fluxo de **recuperar senha**, **auto-cadastro**, **convite** ou **perfil próprio**
   (usuário não edita os próprios dados/senha).

**Qualidade de experiência**
7. **Mensagens de erro do servidor em inglês** numa UI pt-BR, ecoando o valor digitado
   ("CNPJ '...' is invalid.", "User with email '...' already exists.").
8. **Erro genérico "Validation failed"** para senha fraca — o backend sabe a regra exata (RN6),
   a UI não mostra os detalhes; e o hint do formulário diz apenas "Mínimo 8 caracteres" (incompleto).
9. Sem **máscara/validação client-side de CNPJ** — usuário só descobre erro após o submit.
10. **Adicionar permissão exige colar o GUID da empresa** em campo de texto livre — o usuário
    precisa saber um ID interno; deveria ser um seletor.
11. Exclusões usam `confirm()` nativo; sem undo/soft-delete.
12. Elementos decorativos que parecem funcionais: sino de notificações e status
    "Todos os sistemas operacionais" (estático).

**Riscos operacionais**
13. Botão "Entrar com Google" visível, mas o fluxo falha na configuração padrão (redirect_uri com
    host interno — ver skill `arquitetura` §5).
14. Reinício do servidor OAuth desloga todos (tokens invalidados).
15. Deploy de MFE não chega ao usuário sem hard-refresh (cache `immutable` do `remoteEntry.js` —
    skill `arquitetura` §8).

## 5. Perguntas em aberto (para próximas specs)

- Inativar usuário **revoga tokens ativos** ou só bloqueia novo login? (não verificado)
- Qual a semântica de cada papel (`Manager` vs `Writer` vs `Reader`)? Hoje nada na UI muda
  conforme o papel — as permissões são armazenadas mas não aplicadas nas telas.
- Empresa "Inativa" deveria bloquear o quê? (badge existe sem comportamento observável)
- Dados residuais de teste em ambientes: política de limpeza?

## 6. Referências

- Arquitetura e invariantes: skill `arquitetura` · Scaffolding: `SKILL.md` raiz · Testes: skill `qa`
- Journeys e2e cobrindo os fluxos acima: `e2e/tests/journeys/`
