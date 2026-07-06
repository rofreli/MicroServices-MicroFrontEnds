# Plano de publicação — Wiki Notion (produto + processo)

**Status:** ✅ publicada em 2026-07-06.
**Regra:** o repositório é a fonte de verdade (SDD — skill `po` §5.2); a wiki do Notion é a
projeção navegável. Toda edição relevante nasce aqui e é republicada lá.

## Árvore de páginas a criar

```
📕 Plataforma de Leads — Wiki
├── 🏠 Visão Geral                        ← §1–§3 do relatório (sumário, referência real, ideia destilada)
├── 📊 Plano de Negócio
│   ├── Oportunidades e Desafios          ← relatorio-po-plataforma-leads.md §4–§6
│   ├── Caminhos de Viabilidade           ← §7 (MVP manual, métricas de corte)
│   └── Veredito e Perguntas Abertas      ← §10–§11 (com status de cada pergunta)
├── 👥 Papéis da Plataforma
│   ├── Visão e matriz papel × risco      ← §8 (tabelas)
│   └── Fichas detalhadas (10 subpáginas) ← §9.1–§9.10 (uma página por papel)
├── 🗂️ Épicas (database)                  ← epicas-plataforma-leads.md
│   • Propriedades: Status (backlog/spec/aprovada/implementação/verificada),
│     Fase (E0–E7), Gate?, Papéis, Riscos (§), Dependências, Critério de pronto
│   • 8 itens: E0–E7, com E0 marcada como gate das demais
└── ⚙️ Processo (SDD)
    ├── Pipeline: PO → Arquitetura → Implementação → QA   ← skill po §5 (specify/plan/tasks/implement/verify)
    ├── Papel de cada skill                                ← .claude/skills/{po,arquitetura,qa}/SKILL.md (resumos + link)
    ├── Ciclo de vida da spec e rastreabilidade            ← skill po §5.2–§5.3 (specs em docs/specs/)
    └── Dossiês de produto                                 ← .claude/skills/po/products/ (método §2 da skill po)
```

## Passo a passo da publicação (quando o MCP estiver autenticado)

1. Criar a página raiz "Plataforma de Leads — Wiki" no workspace (ou sob a página que o usuário indicar).
2. Criar as páginas de conteúdo convertendo os trechos citados dos arquivos-fonte (não resumir —
   converter, preservando os § para rastreabilidade).
3. Criar o database "Épicas" com as propriedades acima e inserir E0–E7; relacionar cada épica às
   páginas de papéis que a atendem.
4. Interligar: cada página aponta a fonte no repo (caminho + commit) no rodapé.
5. Registrar nesta página o URL raiz da wiki criada (preencher abaixo).

**URL da wiki:** https://app.notion.com/p/395eee72d1ff81fb8414f792b00f28ea

Páginas criadas (2026-07-06):
- 🏠 Visão Geral — https://app.notion.com/p/395eee72d1ff810bbed0d6a1b0771d0a
- 📊 Plano de Negócio — https://app.notion.com/p/395eee72d1ff81488284f550f793cf52
- 👥 Papéis da Plataforma (+ 10 fichas como subpáginas) — https://app.notion.com/p/395eee72d1ff81c88223d44ea5644b59
- 🗂️ Épicas (database, 8 itens E0–E7) — https://app.notion.com/p/ea1c46d738ba49f695135ae381171fa7
- ⚙️ Processo (SDD) — https://app.notion.com/p/395eee72d1ff8140979ecc7a17dbe09b

## Fontes

| Página | Arquivo-fonte |
|--------|---------------|
| Plano de negócio, papéis, veredito | `docs/relatorio-po-plataforma-leads.md` |
| Épicas | `docs/epicas-plataforma-leads.md` |
| Processo SDD | `.claude/skills/po/SKILL.md` (§5), `.claude/skills/arquitetura/SKILL.md`, `.claude/skills/qa/SKILL.md` |
| Dossiê do produto exemplo | `.claude/skills/po/products/business-suite.md` |
