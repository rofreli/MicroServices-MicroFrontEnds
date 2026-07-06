# Épicas — Plataforma de Leads com Fechamento Terceirizado

**Fonte:** [relatorio-po-plataforma-leads.md](relatorio-po-plataforma-leads.md) (§ referenciados abaixo)
**Modelo adotado:** B — marketplace de comissão, vertical primeiro (veredito §11)
**Status:** backlog proposto · pendente de publicação no Notion
**Regra SDD:** nenhuma épica entra em implementação sem spec aprovada em `docs/specs/` (skill `po` §5)

> **E0 é gate das demais:** se as métricas de corte do MVP manual não fecharem, o restante do
> backlog não se justifica (relatório §7.4–7.5).

---

## E0 — Validação de mercado (MVP manual, sem plataforma)

**Objetivo:** provar que o modelo de comissão fecha o ciclo no vertical escolhido antes de
construir software.
**Entregas:** vertical escolhido com justificativa (§7.2); 100 leads montados manualmente
(pipeline CNPJ + IA); 3–5 closers parceiros com contrato de repasse; extrato do closer, ledger de
comissões e opt-out do titular em planilha (§8.3); relatório de conversão.
**Critério de pronto (gate):** taxa lead→fechamento acordada atingida; **≥ 80% das comissões
capturadas**; closers retornam no mês 2 espontaneamente (§7.5).
**Papéis:** todos, acumulados pelos sócios (§9). **Riscos que endereça:** §5.1 (choke point),
§5.3 (qualidade real). **Dependências:** resposta às perguntas §10 (em especial a nº 1).

## E1 — Fundação de dados LGPD-limpa

**Objetivo:** pipeline de captação sustentável sobre fontes legais — sem scraping de rede social
como pilar (§5.2, §7.3).
**Entregas:** ingestão da base CNPJ da Receita (atualização mensal) + dados abertos setoriais;
dedup e normalização; inventário fonte × campo × base legal; fluxo de aprovação de fonte com veto
do DPO (§9.9); quarentena de dados suspeitos.
**Critério de pronto:** 100% dos campos entregues com origem e base legal documentadas; fonte não
aprovada é tecnicamente bloqueada do pipeline.
**Papéis:** curadoria (§9.7), DPO (§9.9). **Riscos:** LGPD/ANPD, due diligence de clientes.

## E2 — Motor de qualificação por IA

**Objetivo:** transformar cadastro em prioridade de abordagem — score de *fit* + sinais de
*intenção*, realimentado pelos desfechos reais (§4.2, §5.3).
**Entregas:** score por segmento/ICP; enriquecimento validado (WhatsApp/e-mail via canais
oficiais); feedback loop desfecho→score; painel de conversão por fonte para a curadoria (§9.7).
**Critério de pronto:** score comprovadamente melhor que ordenação aleatória na conversão do
vertical (medido no piloto E0/E5); % de leads contestados como inválidos abaixo do teto contratual.
**Papéis:** curadoria (§9.7), SDR/closer como sensores (§9.1, §9.4). **Riscos:** "qualificado"
sem lastro (§5.3).

## E3 — Choke point financeiro (ledger de comissões e conciliação)

**Objetivo:** operacionalizar a captura da comissão — a viabilidade do modelo B em software (§5.1).
**Entregas:** contrato de canal padrão (comissão-mãe paga à plataforma — §9.2); ledger
venda↔lead↔closer↔take rate↔repasse; conciliação com a parceira; alertas de leakage (lead
"perdido" que virou cliente da parceira — §9.8); régua de cobrança de divergências.
**Critério de pronto:** todo real de comissão rastreável de ponta a ponta; divergência detectada
em ≤ 30 dias; % de captura ≥ meta do E0.
**Papéis:** financeiro/conciliação (§9.8), empresa parceira (§9.2), admin (§9.6).
**Riscos:** §5.1 — **a épica que decide o negócio**.

## E4 — Marketplace do closer

**Objetivo:** o lado da demanda operando com confiança: fila justa, SLA e extrato transparente
(§9.1).
**Entregas:** cadastro e homologação (documentos/certificações do vertical); fila de leads com
exclusividade e SLA de 1º contato; CRM leve de cadência; registro de desfecho obrigatório;
extrato de comissões; fluxo de contestação de lead com efeito no extrato (§9.10).
**Critério de pronto:** closer opera do onboarding ao repasse sem intervenção manual; retenção
de closers ativos no mês 2 ≥ meta definida no E0.
**Papéis:** closer (§9.1), suporte/CS (§9.10). **Riscos:** churn da demanda, disputas.

## E5 — Portal da empresa parceira

**Objetivo:** dar à dona do produto visibilidade e conciliação para mantê-la dentro do fluxo
financeiro (§9.2).
**Entregas:** dashboard do funil (propostas, fechamentos, cancelamentos precoces); conciliação de
comissões; gestão de homologação de closers; isolamento total entre parceiras.
**Critério de pronto:** parceira acompanha e concilia sem pedir planilha; segunda parceira do
mesmo vertical onboardada sem código novo.
**Papéis:** empresa parceira (§9.2), financeiro (§9.8). **Riscos:** parceira pagar direto ao
closer (§9.2 — "fim silencioso do take rate").

## E6 — Compliance e direitos do titular

**Objetivo:** tratar o titular de dados como papel de primeira classe (§9.5) — o que passa em
due diligence e mantém os canais de contato vivos.
**Entregas:** canal público do titular (acesso/correção/exclusão/opt-out, sem login); **supressão
que propaga** para listas já distribuídas; fila do DPO com prazos legais; abordagem via canais
oficiais (WhatsApp Business API, templates aprovados — §5.2); trilha de auditoria de tratamento.
**Critério de pronto:** reincidência de contato pós-opt-out = zero; solicitações de titular
atendidas no prazo legal; zero abordagem fora de canal oficial.
**Papéis:** titular (§9.5), DPO (§9.9). **Riscos:** ANPD, banimento Meta, queima do vertical.

## E7 — Plataforma e operação (RBAC, parametrização, auditoria)

**Objetivo:** operar o negócio sem deploy e sem mexer em banco (§9.6), com os acessos por escopo
descritos nas fichas §9.
**Entregas:** RBAC módulo × papel × escopo (closer só vê os próprios leads; parceira só o próprio
funil) — mapeado no modelo de permissões do monorepo (skill `arquitetura` §5); parametrização de
take rate/SLA/cotas por vertical; trilha de auditoria administrativa; onboarding assistido
(§9.10).
**Critério de pronto:** nenhum ajuste comercial exige deploy; toda ação administrativa auditável
(quem/quando/o quê).
**Papéis:** admin (§9.6), suporte/CS (§9.10). **Riscos:** disputa sem prova, operação manual
insustentável.

---

## Sequenciamento e dependências

```
E0 (gate) ──► E1 ──► E2 ──► E4 ──► (piloto assistido no vertical)
                 └──► E3 ──► E5
E6: inicia com E1 (base legal nasce com a base de dados)
E7: transversal — primeira entrega junto de E4 (RBAC do closer)
```

**Fora de escopo desta fase** (§11): motor horizontal multi-segmento; crawler de
LinkedIn/Instagram/Maps; modelo A (venda de listas) como produto principal.
