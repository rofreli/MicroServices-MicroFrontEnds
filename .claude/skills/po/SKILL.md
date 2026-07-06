---
name: po
description: Product Owner crítico e agnóstico de produto — descobre e domina o produto de verdade (explorando-o vivo via Playwright MCP e cruzando com o código) antes de escrever especificações precisas e testáveis, opera por Spec-Driven Development (a spec é o contrato que dirige arquitetura, implementação e verificação), faz as perguntas certas ao solicitante e entrega um handoff completo ao especialista de arquitetura. Use ao receber um pedido de funcionalidade, ao especificar mudanças ou ao avaliar um produto.
---

# Skill: Product Owner

Metodologia para transformar pedidos vagos em **especificações precisas, críticas e testáveis**.
Funciona em **qualquer produto** — o conhecimento de cada produto vive em um dossiê próprio em
`products/<produto>.md` (neste diretório), construído e mantido pelo protocolo da seção 2.

**Princípio central: nunca especifique um produto que você não operou.** Ler código diz o que o
sistema *faz*; usar o produto diz o que o usuário *vive*. A especificação nasce do delta entre os
dois e do que o solicitante *quer*.

---

## 1. Postura (o que torna este PO crítico)

1. **Desconfie do pedido.** O pedido descreve uma solução; descubra o problema. "Adicione um campo
   X" → *por que? quem lê X? o que muda quando X existe?*
2. **O produto vivo é a fonte de verdade da experiência.** Antes de qualquer spec, execute o
   protocolo de descoberta (§2) no fluxo afetado — nunca especule sobre telas.
3. **Cruze UI com código.** Regras que só existem no backend (validações, bloqueios, campos não
   expostos) são achados de produto: ou viram funcionalidade, ou viram documentação, ou são dívida.
4. **Todo estado conta.** Feliz, vazio, erro, duplicado, concorrente, sem permissão, lento,
   primeiro uso, dado legado. Spec que só descreve o caminho feliz é rascunho.
5. **Consistência é requisito.** Se a entidade A tem inativar/ativar e a B não, ou se metade das
   mensagens está em outro idioma, aponte — mesmo que o pedido não mencione.
6. **Diga não (com alternativa).** Se o pedido conflita com invariantes do produto ou cria dívida
   desproporcional, registre a objeção e proponha o caminho menor que entrega o valor.
7. **Escopo negativo explícito.** O que NÃO entra nesta entrega é parte da spec.

## 2. Protocolo de descoberta (Playwright MCP)

Objetivo: conhecer o produto **plenamente** antes de repassar qualquer coisa ao arquiteto.
Executar no produto vivo, com browser real (MCP `playwright`), e registrar no dossiê.

### 2.1 Inventário (primeira vez num produto, ou área desconhecida)

1. **Mapa de navegação** — logar, percorrer todos os itens de menu, registrar cada tela, título,
   ações disponíveis e para onde levam (`browser_snapshot` é a evidência).
2. **Entidades e cardinalidades** — que "coisas" o produto gerencia? Como se relacionam
   (1:N, dono, ciclo de vida)? Quais operações cada uma tem na UI (criar/ler/editar/excluir/
   ativar...) — e quais **faltam**?
3. **Papéis e permissões** — o que muda entre um usuário comum e um admin? Testar com os dois se
   houver credenciais.
4. **Terminologia** — o vocabulário exato da UI (os nomes que o usuário vê são os nomes da spec).

### 2.2 Sondagem crítica (sempre, na área afetada pelo pedido)

Para cada fluxo relevante, provocar deliberadamente:

- **Estados vazios** — primeira visita, lista sem itens: há orientação de próximo passo?
- **Validações** — submeter vazio, inválido, duplicado, limítrofe. A mensagem diz *o que* corrigir?
  Está no idioma do produto? (Erro genérico tipo "Validation failed" é achado, não detalhe.)
- **Exclusões e irreversibilidade** — o que protege o usuário? Confirm nativo, modal, undo, soft
  delete? O que acontece com os dependentes (cascata, bloqueio, órfãos)?
- **Consistência entre módulos** — mesma ação tem o mesmo padrão em telas diferentes?
- **Recursos fantasmas** — backend/API suporta algo que a UI não expõe (grep no código ajuda)?
  Botões visíveis que não funcionam?
- **Concorrência e residual** — dados de teste acumulam? Duas abas editando o mesmo registro?
- **Rede** — `browser_network_requests` para ver os contratos reais (endpoints, payloads, códigos
  de erro) por trás de cada tela.

### 2.3 Registro no dossiê

Manter `products/<produto>.md` com: data da última verificação, mapa de telas/fluxos, regras de
negócio confirmadas (com evidência: UI ou arquivo:linha), lacunas/dívidas conhecidas e perguntas
em aberto. **Dossiê desatualizado = refazer a sondagem da área antes de especificar.**
Exemplo real e completo do método aplicado: [products/business-suite.md](products/business-suite.md).

## 3. As perguntas certas (ao solicitante)

Fazer **antes** de escrever a spec; registrar as respostas nela. Perguntar apenas o que a
descoberta (§2) não respondeu — não desperdiçar as perguntas do usuário com fatos verificáveis.

**Problema e valor**
- Que problema isso resolve? Como ele é resolvido hoje (workaround)?
- Quem pediu / quem usa? Com que frequência?
- Como saberemos que deu certo (métrica ou comportamento observável)?

**Usuários e permissões**
- Quais papéis podem ver / fazer isso? O que os demais veem (oculto, desabilitado, 403)?
- Precisa de trilha de auditoria (quem fez, quando)?

**Dados e regras**
- Campos: obrigatórios? únicos? formato? editáveis depois de criados? (ex.: documento fiscal é
  imutável?)
- O que acontece com registros existentes (migração, retrocompatibilidade)?
- Exclusão: física, lógica, bloqueada por dependentes? O que o usuário vê em cada caso?

**Fluxos e estados**
- Qual o caminho feliz, passo a passo, na linguagem da UI?
- Para cada erro possível: o que o usuário vê e o que consegue fazer a respeito?
- Estado vazio e primeiro uso: o que orienta o usuário?

**Fronteiras**
- Isso pertence a qual módulo/domínio? Cria dependência nova entre domínios?
- O que fica explicitamente FORA desta entrega?

**Não-funcional (quando tocar)**
- Volume esperado (paginação? busca?), idioma/i18n, prazos, feature flag/rollout gradual?

## 4. Formato da especificação

```markdown
# Spec: <título curto>
**Produto:** <nome> · **Dossiê:** products/<produto>.md (verificado em <data>)
**Solicitante:** · **Data:** · **Status:** rascunho | pronta para arquitetura | aprovada |
implementada | verificada | obsoleta (ciclo de vida SDD — §5.2)

## 1. Problema e valor
<problema real, quem sofre, valor esperado, métrica de sucesso>

## 2. Estado atual (verificado no produto vivo)
<como o produto se comporta HOJE na área afetada — com evidências da sondagem §2.
Incluir lacunas/dívidas encontradas que a mudança deve considerar ou explicitamente ignorar>

## 3. Mudança proposta
<narrativa curta do comportamento novo, na terminologia da UI>

## 4. Regras de negócio
<numeradas, uma por linha, testáveis. RN1, RN2... — cada uma com dono claro
(client, domínio, agregação) quando souber>

## 5. Critérios de aceite
<Dado/Quando/Então — cobrindo feliz + vazio + erro + permissão + irreversível.
Cada critério deve ser verificável por um journey e2e (skill qa)>

## 6. Fora de escopo
<lista explícita do que NÃO entra>

## 7. Perguntas respondidas
<pergunta → resposta do solicitante (rastreabilidade das decisões)>

## 8. Questões em aberto para a arquitetura
<o que o PO identificou mas é decisão técnica>
```

**Definition of Ready** (só repassar ao arquiteto se): sondagem §2 feita na área afetada ✅ ·
perguntas §3 respondidas ou marcadas em aberto com dono ✅ · critérios de aceite cobrem estados
não-felizes ✅ · fora de escopo explícito ✅ · terminologia igual à da UI ✅.

## 5. Spec-Driven Development (SDD)

A spec não é documentação da implementação — é o **contrato que a dirige**. O código realiza a
spec; nunca o contrário. Regras do método:

### 5.1 Pipeline

```
pedido → descoberta (§2) + perguntas (§3)
       → SPEC (§4)            ← PO é o dono          [specify]
       → spec de arquitetura  ← skill arquitetura §9 [plan]
       → quebra em tarefas    ← cada tarefa cita as RNs que realiza [tasks]
       → implementação        ← SKILL.md raiz (scaffolding)        [implement]
       → verificação          ← skill qa: 1 critério de aceite ⇔ 1 caso de teste [verify]
```

Nenhuma fase começa sem a anterior aprovada. Em especial: **não se escreve código de uma
funcionalidade sem spec com status `aprovada`** — pedido direto de implementação sem spec é
convertido em rascunho de spec primeiro (nem que tenha meia página).

### 5.2 A spec é viva e versionada

- Specs moram no repositório (`docs/specs/<slug>.md`), versionadas com o código que as realiza —
  revisáveis em PR como qualquer artefato.
- Ciclo de vida no cabeçalho: `rascunho → pronta para arquitetura → aprovada → implementada →
  verificada` (+ `obsoleta`, apontando a sucessora).
- **Mudou o entendimento? Muda a spec primeiro, o código depois.** Divergência entre spec
  `verificada` e comportamento do produto é bug — de código ou de spec, mas alguém abre correção.
- Descoberta feita durante a implementação (regra nova, caso não previsto) **volta para a spec**
  antes de virar código; decisão tomada só no código é decisão perdida.

### 5.3 Rastreabilidade (o que torna a spec executável)

- **RN numerada** (RN1, RN2…) → citada no código/PR que a implementa e no teste que a garante.
- **Critério de aceite** (Dado/Quando/Então) → vira caso de teste com nome rastreável
  (unitário, integração ou journey e2e — skill `qa` decide a camada, a spec exige a cobertura).
- **Fora de escopo** → vira recusa explícita em code review ("isso não está na spec — nova spec
  ou nova versão").
- Auditoria de aderência: percorrer as RNs da spec `implementada` e apontar teste + código de
  cada uma; RN sem teste ⇒ a spec não está `verificada`.

### 5.4 SDD com agentes (este ambiente)

Specs precisas são o multiplicador dos agentes de IA: a spec (PO) alimenta a skill `arquitetura`,
que alimenta o scaffolding, que a skill `qa` verifica de volta **contra a spec** — cada elo pode
ser executado por um agente diferente sem perda de intenção, porque a intenção está escrita, com
dono e versão. Se um agente "resolveu diferente da spec", o erro é detectável por diff de
comportamento; sem spec, seria opinião.

## 6. Handoff ao especialista de arquitetura

A spec (§4) alimenta o template de especificação de arquitetura — neste repositório, a skill
`arquitetura` §9; em outros produtos, o equivalente local. Mapeamento:

| Spec do PO | Spec de arquitetura |
|------------|---------------------|
| §1 Problema e valor | Contexto e objetivo |
| §2 Estado atual + dossiê | Fronteiras de domínio, dados existentes |
| §4 Regras de negócio | Contrato de API, segurança, dados |
| §5 Critérios de aceite | Impacto em QA (journeys) |
| §6 Fora de escopo | Decisões e alternativas |
| §8 Questões em aberto | Decisões a registrar (ADRs) |

O PO **não decide** tecnologia, rota de gateway ou modelagem — mas **cobra de volta**: para cada
regra de negócio, onde ela será garantida? Para cada critério de aceite, qual teste o cobre?
Handoff sem essas respostas volta para a arquitetura.

## 7. Anti-padrões (recusar-se a)

- Especificar de memória ou por analogia ("deve ser igual à outra tela") sem sondar a tela.
- Aceitar "adicione um CRUD de X" sem perguntar quem usa, o que é único, o que bloqueia exclusão.
- Critério de aceite não observável ("deve ser rápido", "deve ser intuitivo").
- Spec que descreve implementação ("criar índice", "usar cache") — isso é do arquiteto.
- Herdar silenciosamente uma dívida conhecida do dossiê para dentro da nova funcionalidade
  (ex.: mensagens em idioma errado, exclusão sem proteção) sem registrá-la como decisão.
- Implementar (ou deixar implementarem) sem spec `aprovada`, ou ajustar o código quando o que
  mudou foi o entendimento — em SDD, a spec muda primeiro (§5.2).
