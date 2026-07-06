# Relatório de PO — Plataforma de Prospecção, Qualificação e Fechamento de Leads

**Data:** 2026-07-06 · **Método:** skill `po` — sondagem do produto de referência vivo
(infinityleads.app.br via browser real) + destilação crítica da transcrição da conversa.
**Status:** análise de viabilidade (pré-spec). **Veredito:** GO condicional — ver §11.

---

## 1. Sumário executivo

A dor citada é real e grande (CAC e prospecção manual), mas a conversa mistura **dois negócios
diferentes** sob o mesmo nome — e o site citado como referência **não opera o modelo descrito**:

- **Modelo A — SaaS de dados/leads** (o que a infinityleads.app.br realmente é): vende listas B2B
  enriquecidas por assinatura. Mercado validado, porém **comoditizado** — o próprio site cobra
  **R$ 0,20–0,27 por lead**. Entrar aqui exige um diferencial que lista não tem.
- **Modelo B — marketplace de fechamento** (o que a transcrição descreve: "30 leads/mês → R$ 30 mil
  de comissão via corretores autônomos"): margem muito maior, porém com um **problema estrutural
  de enforcement** — só funciona quando a plataforma **controla o fluxo do dinheiro** da comissão,
  como acontece no ramo securitário. Horizontal ("qualquer base, qualquer segmento") esse controle
  não existe, e o closer fecha por fora.

A oportunidade existe, mas não é "um motor de crawler + IA para qualquer segmento". É **escolher
um dos dois modelos** e, no caso do B, **um vertical por vez, com ponto de estrangulamento do
pagamento**. O crawler é a parte mais fácil — e a menos defensável — do plano.

## 2. O que foi verificado no produto de referência (evidências)

Sondagem em 2026-07-06, browser real:

| Fato | Evidência |
|------|-----------|
| Posicionamento: "big data de leads B2B" self-service, com CRM próprio | landing: "Somos uma empresa de big data especializada em geração de leads B2B" |
| Preços: Start R$0 (10 leads/mês) · Basic R$67 (250) · Premium R$247 (1.000) · Enterprise R$497 (2.500) | seção de planos; "R$ 0,20–0,27 por lead" exibido pelo próprio site |
| Promessa: "decisores reais com WhatsApp e e-mail validados", "80 pontos de dados de múltiplas fontes" | hero + FAQ |
| Público: times B2B/SDRs, agências, consultorias, PMEs, representantes | seção "Para quem é" |
| Postura LGPD declarada: leads = "informações públicas de **empresas** (nome, endereço, telefone, site, redes sociais, CNPJ) coletadas de fontes públicas" | política de privacidade (jun/2026) |
| API/Webhooks "em breve"; pagamento via Mercado Pago | plano Enterprise; política |

**Conclusão crítica:** não há na infinityleads.app.br nada de marketplace de corretores, fatia de
comissão ou distribuição de leads com meta de retorno. O modelo "30 leads → R$ 30k" descrito na
conversa é de **outra operação** (provavelmente uma corretora/assessoria do ramo securitário com
nome parecido). A referência precisa ser corrigida antes de qualquer decisão — hoje o plano está
ancorado numa empresa que faz outra coisa.

## 3. A ideia da transcrição, destilada

Três camadas + um modelo de receita:

1. **Captação** — crawler de "bases públicas": Google Maps, LinkedIn, Instagram → capturar dados
   de potenciais clientes (inspirado em vídeo de YouTube: agente de IA varrendo Google Maps de um
   bairro e gerando planilha).
2. **Qualificação** — camada de IA que classifica/pontua a base.
3. **Fechamento** — terceirizar para vendedores/corretores autônomos cadastrados.
4. **Receita** — % da comissão dos contratos fechados (referência citada: 30 leads/mês entregues,
   ~R$ 30 mil/mês de comissionamento retornado à plataforma).

Ambição declarada: funcionar "com qualquer base ou segmento" (horizontal) e "de forma escalável".

## 4. Oportunidades (o que joga a favor)

1. **A dor é universal e cara.** Prospecção B2B manual consome o tempo mais caro da empresa
   (vendedor). Disposição a pagar comprovada — o mercado brasileiro já sustenta Speedio,
   Econodata, Casa dos Dados etc.
2. **IA mudou a economia da qualificação.** SDR virtual (enriquecer, pontuar, redigir abordagem,
   agendar) hoje custa centavos por lead — há espaço para entregar **intenção**, não só cadastro,
   que é onde as bases atuais são fracas.
3. **Brasil tem dados públicos B2B ricos e legais**: base de CNPJ da Receita (aberta, atualizada
   mensalmente), CNAE, porte, sócios, dados abertos setoriais — dá para montar firmografia sem
   raspar rede social.
4. **Modelo de performance alinha incentivos** (no B): a plataforma só ganha se o lead converte —
   argumento de venda fortíssimo contra assinatura de lista.
5. **Exército de vendedores autônomos existe** (corretores, representantes, consultores) e é mal
   abastecido de pipeline — o lado da oferta do marketplace é recrutável.
6. **O incumbente citado compete por preço, não por qualidade** (R$ 0,20/lead) — sinal de espaço
   na faixa premium (lead com intenção/reunião agendada vale 100–1000× isso).

## 5. Desafios (onde o plano quebra, se quebrar)

### 5.1 O problema estrutural do Modelo B: capturar a comissão

O modelo securitário funciona porque a plataforma/corretora master **está no meio do dinheiro**:
a seguradora paga a comissão à corretora, que repassa ao autônomo. Há choke point contratual e
regulado (SUSEP). **"Qualquer segmento" não tem esse choke point** — se a plataforma entrega o
lead e o fechamento acontece fora dela, não há como cobrar a fatia. O vendedor racional fecha por
fora na segunda venda. Sem resposta convincente para *"por onde passa o dinheiro?"*, o Modelo B
horizontal é inviável — vira, na prática, o Modelo A com marketing diferente.

### 5.2 Legal e de plataforma (sério, não detalhe)

- **LGPD:** dados de **empresa** (CNPJ, telefone comercial) são o terreno seguro — exatamente a
  postura da política da InfinityLeads real. **Dados de pessoas** (decisor com WhatsApp pessoal,
  perfil de Instagram/LinkedIn) são dados pessoais: prospecção fria exige base legal (legítimo
  interesse com teste de balanceamento, opt-out funcional, canal de titular). Raspar perfis
  sociais e revender é o cenário com risco real de sanção ANPD e — pior para vendas — de cliente
  enterprise recusar o fornecedor na due diligence.
- **ToS das fontes:** LinkedIn, Instagram e **Google Maps proíbem scraping**. O vídeo de YouTube
  citado na conversa é um truque de demonstração, não uma operação: em escala vêm bloqueios,
  captchas, banimento de contas e custo permanente de manutenção de crawler (proxies, evasão) —
  além de risco jurídico. **Crawler de rede social não pode ser a fundação do negócio.**
- **WhatsApp:** abordagem em massa fora da API oficial ⇒ banimento de números pela Meta. Cadência
  de contato precisa ser desenhada dentro das regras (API oficial, templates aprovados) desde o dia 1.

### 5.3 Produto e mercado

- **"Lead qualificado" é promessa que quebra rápido.** IA sobre dados raspados entrega **fit**
  (a empresa parece certa), não **intenção** (ela quer comprar agora). Se 30 leads/mês não geram
  fechamento, o closer some no mês 2 — churn de marketplace é imperdoável.
- **Frio dos dois lados:** sem leads bons não há closers; sem closers não há receita para melhorar
  os leads. Começar horizontal multiplica esse problema por N segmentos.
- **Concorrência assimétrica no Modelo A:** Speedio/Econodata/Casa dos Dados (dados BR),
  Apollo/Lusha (global), e um oceano de agências de outbound. Lista B2B é commodity — o preço da
  própria referência (R$ 0,20/lead) é a prova.
- **Métrica da conversa não generaliza:** "R$ 30 mil/mês de comissão sobre 30 leads" implica
  ticket e recorrência do securitário (plano de saúde tem comissão alta e recorrente). Padaria do
  bairro (o exemplo do vídeo) não paga isso — o exemplo inspirador e a meta financeira citada são
  de universos incompatíveis.

## 6. Economia unitária (esboço honesto)

**Modelo A (SaaS de dados):** preço de mercado R$ 0,20–0,30/lead ⇒ receita por cliente Basic
~R$ 67/mês. Com custo de validação (WhatsApp/e-mail), infra de coleta e suporte, a margem só
fecha com volume alto e churn baixo — guerra de preço contra players estabelecidos. **Só entrar
com diferencial de intenção/automação** (ex.: reunião agendada, não linha de planilha).

**Modelo B (comissão):** referência securitária: ~R$ 1.000 de comissão implícita por lead
entregue (30 leads ↔ R$ 30k). Se a plataforma fica com 20–30%, cada closer ativo vale
R$ 6–9k/mês de receita — 100 closers ativos ≈ R$ 0,6–0,9M/mês. **Mas** cada R$ 1 dessa conta
depende do §5.1 (choke point) e da qualidade real do lead (§5.3). A conta é linda e frágil.

## 7. Caminhos de viabilidade (recomendação do PO)

1. **Decidir o modelo antes de escrever uma linha de código.** A (vender dados/ferramenta) e B
   (vender fechamento) têm produto, cliente, risco e go-to-market diferentes. "Os dois" = nenhum.
2. **Se Modelo B (recomendado pela margem): vertical primeiro, horizontal nunca no MVP.**
   Critérios para escolher o vertical: (a) comissão alta e **recorrente**; (b) pagamento da
   comissão **formalizável via plataforma** (ser a corretora/representante master, ou integrar o
   pagamento); (c) exército de autônomos fragmentado; (d) dados de prospecção legalmente
   acessíveis. Candidatos a validar: seguros/planos (replicar o modelo em outra região/nicho),
   energia solar, consórcio, crédito/maquininhas, telecom B2B.
3. **Fundação de dados legal:** base CNPJ da Receita + dados abertos + enriquecimento consentido
   (formulários, parcerias) — crawler de rede social no máximo como enriquecimento pontual e
   revisável, nunca como pilar.
4. **MVP de validação sem plataforma** (4–8 semanas, quase sem código): escolher 1 vertical,
   montar 100 leads com o pipeline manual+IA, entregar a 3–5 closers parceiros com contrato de
   repasse, medir conversão e **se a comissão volta**. Se o repasse não voltar nem com 5 parceiros
   escolhidos a dedo, o §5.1 está confirmado e o modelo B morre barato.
5. **Métricas de corte (go/no-go do MVP):** taxa lead→fechamento ≥ X% definida com o closer;
   ≥ 80% das comissões efetivamente capturadas; closer retorna no mês 2 sem ser cobrado.
6. **Se Modelo A:** não competir por preço; competir por **resultado por lead** (IA que valida,
   aborda e agenda) e por nicho (dados profundos de 1 setor > dados rasos de todos).

## 8. Papéis que permeiam a plataforma

Papéis desenhados a partir dos riscos deste relatório — cada um existe para responder a um
problema concreto, não por organograma. Marcados por modelo (A = SaaS de dados, B = marketplace
de comissão).

### 8.1 Papéis externos (o mercado)

| Papel | Modelo | O que faz | O que exige do produto | Risco que endereça |
|-------|--------|-----------|------------------------|--------------------|
| **Closer / vendedor autônomo** (corretor, representante) | B | Recebe a fila de leads, executa a cadência, reporta status de cada lead, repassa a comissão acordada | Fila com SLA e exclusividade, CRM leve, extrato transparente de comissões, regras claras de "lead válido" | Churn do lado da demanda (§5.3): sem confiança no lead e no extrato, some no mês 2 |
| **Empresa parceira / dona do produto vendido** (seguradora, integrador solar, administradora de consórcio) | B | Fornece o produto, paga a comissão-mãe, homologa closers | Visibilidade do funil, conciliação de vendas × comissões, contrato de repasse via plataforma | **É o choke point do dinheiro (§5.1)** — sem ela dentro, não há captura de comissão |
| **Gestor comercial / assinante** | A | Contrata o plano, define ICP, gerencia consumo de créditos e o time | Gestão de assinatura, filtros de segmentação, controle de usuários e cotas | Receita do modelo A |
| **SDR / vendedor interno do assinante** | A | Consome as listas, aborda, exporta para o CRM da empresa | Exportação, integrações (API/webhook), validação de contato em tempo real | Qualidade percebida (§5.3) — é quem sente lead frio primeiro |
| **Lead / titular de dados** | A e B | Não usa a plataforma, mas é a matéria-prima dela — e tem direitos | Canal do titular (acesso/correção/exclusão), opt-out que **propaga** para listas já entregues, supressão permanente | LGPD (§5.2): tratar o titular como papel de primeira classe é o que passa em due diligence |

### 8.2 Papéis internos (a operação)

| Papel | Modelo | O que faz | O que exige do produto | Risco que endereça |
|-------|--------|-----------|------------------------|--------------------|
| **Administrador da plataforma** | A e B | Contas, planos, papéis, parâmetros de negócio (take rate, SLAs, cotas) | RBAC completo, trilha de auditoria de ações administrativas | Operação e segurança |
| **Curador de dados / operações de leads** | A e B | Gere fontes e pipelines de coleta, dedup, score, redistribui leads queimados, mede qualidade por fonte | Painel de qualidade (conversão por fonte/segmento), quarentena de dados suspeitos, versionamento de fonte | Qualidade (§5.3) — "qualificado" é uma operação contínua, não um modelo de IA estático |
| **Financeiro / conciliação** | B | Concilia venda → comissão-mãe → take rate → repasse ao closer; cobra repasse em atraso; detecta fechamento por fora | Ledger de comissões por contrato, integração com o pagamento da empresa parceira, alertas de leakage (lead marcado "perdido" que virou cliente da parceira) | **Enforcement (§5.1) operacionalizado** — este papel é o motivo de o modelo B ser viável ou não |
| **DPO / compliance** | A e B | Define base legal por campo coletado, responde ANPD e titulares, aprova novas fontes de dados | Inventário de dados por origem e base legal, relatórios de atendimento ao titular, bloqueio de fonte não aprovada | Legal (§5.2) — sem este papel com poder de veto, a curadoria vai raspar o que converte |
| **Suporte / customer success** | A e B | Onboarding de closers/assinantes, arbitra disputas de "lead inválido", monitora ativação | Fluxo formal de contestação de lead com prazo e efeito no extrato, playbooks de onboarding | Confiança dos dois lados do marketplace — disputa mal resolvida derruba os dois |

### 8.3 Leitura crítica dos papéis

- **Os dois papéis que decidem o negócio no modelo B não são técnicos:** *empresa parceira*
  (traz o choke point) e *financeiro/conciliação* (opera-o). Se o MVP (§7.4) não conseguir
  preencher esses dois papéis com gente/contratos reais, a plataforma não tem o que automatizar.
- **O titular de dados é papel, não linha de banco.** Opt-out que não propaga para listas já
  exportadas é passivo jurídico acumulando silenciosamente.
- **Curadoria e compliance ficam em tensão proposital** (um quer converter, o outro pode vetar
  fonte); a plataforma deve registrar essa decisão, não escondê-la.
- No MVP manual (§7.4), uma pessoa acumula vários papéis — mas o **extrato do closer**, o
  **ledger de comissões** e o **opt-out do titular** já precisam existir desde o primeiro dia,
  nem que seja em planilha: são os três artefatos que provam o modelo.
- Para a spec futura: esses papéis mapeiam direto para o modelo de permissões por escopo já usado
  no monorepo (módulo × papel × empresa — ver skill `arquitetura` §5), o que torna esta plataforma
  um candidato natural a novo domínio da stack existente.

## 9. Descrição detalhada de cada papel

Formato: quem é · o que quer · jornada e responsabilidades · acessos na plataforma · KPIs ·
o que acontece se o papel falhar.

### 9.1 Closer / vendedor autônomo (modelo B)

**Quem é:** corretor, representante ou consultor comercial autônomo, geralmente PJ/MEI, que vive
de comissão e não tem pipeline próprio constante. Perfil heterogêneo: do especialista com carteira
ao iniciante sem método.
**O que quer:** previsibilidade de renda — leads que atendem, prazo para trabalhar cada um e
clareza absoluta de quanto vai receber e quando.
**Jornada:** cadastro → homologação (documentos, certificações do vertical, aceite do contrato de
repasse) → recebe fila de leads com SLA → registra cada interação e o desfecho (ganho/perdido/
inválido) → fechamento formalizado → recebe o repasse e vê o take rate descontado no extrato.
**Acessos:** apenas os próprios leads (escopo individual); CRM leve de cadência; extrato de
comissões; abertura de contestação de lead. **Nunca** vê a base bruta nem leads de outros closers.
**KPIs:** taxa de contato, lead→fechamento, tempo até 1º contato (SLA), repasse em dia, retenção
mês a mês.
**Se falhar:** leads apodrecem na fila e a conversão medida mente sobre a qualidade da base; ou
pior — fecha por fora (leakage), o que é falha do desenho do choke point (§5.1), não só do closer.

### 9.2 Empresa parceira / dona do produto (modelo B)

**Quem é:** a operadora/seguradora/integradora/administradora cujo produto é vendido — quem paga
a comissão-mãe. É contrato de canal, não usuário casual.
**O que quer:** volume de vendas com qualidade (baixo cancelamento/chargeback) sem precisar
gerenciar um exército de autônomos um a um.
**Jornada:** contrato de canal com a plataforma (comissão-mãe, critérios de homologação de
closers, SLA de qualidade) → recebe propostas/fechamentos → paga a comissão **para a plataforma**
(este é o choke point; se pagar direto ao closer, o modelo desmorona) → acompanha funil e
conciliação.
**Acessos:** dashboard do próprio funil (propostas, fechamentos, cancelamentos), conciliação de
comissões, gestão de homologação; **nunca** os dados de leads de outros parceiros.
**KPIs:** vendas/mês pelo canal, taxa de cancelamento precoce, prazo de pagamento da comissão-mãe.
**Se falhar:** sem parceira dentro, não há o que repassar — a plataforma vira lista (modelo A) sem
querer. Parceira que paga direto ao closer é o fim silencioso do take rate.

### 9.3 Gestor comercial / assinante (modelo A)

**Quem é:** dono de PME, head de vendas ou dono de agência que contrata a plataforma para
abastecer o time.
**O que quer:** encher o funil do time com o menor custo por reunião agendada — e provar ROI da
assinatura para si mesmo todo mês.
**Jornada:** cria conta → define ICP (segmento, porte, região) → contrata plano → distribui cotas
ao time → acompanha consumo e conversão → decide renovar (ou não) com base no que converteu.
**Acessos:** gestão da assinatura e usuários, definição de filtros/ICP, relatórios de consumo e
qualidade, exportações e integrações.
**KPIs:** custo por lead útil, leads→reuniões, churn da assinatura.
**Se falhar (ou se o produto falhar com ele):** churn no 2º mês — em SaaS de R$ 67–497/mês não há
margem para recuperar cliente com CS caro; a retenção depende de qualidade percebida imediata.

### 9.4 SDR / vendedor interno do assinante (modelo A)

**Quem é:** quem executa a prospecção no dia a dia com a lista gerada; o usuário mais frequente e
o primeiro a sentir dado ruim.
**O que quer:** contato que atende (WhatsApp/telefone válido), dado que não o faça passar vergonha
e o mínimo de retrabalho de digitação.
**Jornada:** recebe cota → gera/filtra lista → valida e aborda → marca desfecho → exporta para o
CRM da empresa.
**Acessos:** consumo de leads dentro da cota, exportação, click-to-chat; sem acesso à gestão da
assinatura.
**KPIs:** contatos válidos/lista (taxa de bounce), abordagens/dia, feedback de qualidade por lote.
**Se falhar:** é o canal de detecção de qualidade mais rápido que existe — se o produto não coleta
o desfecho que ele registra, a curadoria (9.7) fica cega.

### 9.5 Lead / titular de dados (modelos A e B)

**Quem é:** a pessoa (decisor) e a empresa prospectada. Não é usuário, mas a LGPD lhe dá direitos
que o produto precisa operacionalizar — e a experiência dele com a abordagem é a reputação da
plataforma.
**O que quer:** não ser importunado; se abordado, que seja relevante; poder dizer "me tirem dessa
lista" **uma vez** e valer para sempre.
**Jornada (involuntária):** coletado de fonte pública → qualificado → distribuído → abordado →
(possivelmente) exerce direito de titular.
**Acessos:** canal público do titular (sem login): acesso, correção, exclusão, opt-out.
**KPIs:** solicitações de titular atendidas no prazo legal, reclamações por 1.000 abordagens,
reincidência de contato pós-opt-out (meta: zero).
**Se falhar:** sanção ANPD, número de WhatsApp banido pela Meta e — antes disso — queima do
vertical: mercado pequeno comenta, e closers/parceiras não querem associar a marca a spam.

### 9.6 Administrador da plataforma (interno)

**Quem é:** operador central do negócio (no início, um dos sócios).
**O que quer:** operar contas, planos e parâmetros sem depender de deploy ou de mexer no banco.
**Jornada:** gerencia contas e papéis → configura parâmetros de negócio (take rate por vertical,
SLAs, cotas, preços) → acompanha saúde da operação → intervém em exceções.
**Acessos:** RBAC completo, parametrização de negócio, visão global — com **trilha de auditoria**
de cada ação administrativa (quem mudou take rate, quem reatribuiu lead).
**KPIs:** tempo de resolução de exceções, zero intervenções diretas em banco.
**Se falhar:** parâmetros de negócio hardcoded viram deploy para cada ajuste comercial — e ajuste
de take rate sem auditoria vira disputa com closer sem prova.

### 9.7 Curador de dados / operações de leads (interno)

**Quem é:** o dono da matéria-prima — analista de dados/operações que responde pela pergunta "por
que a conversão caiu?". No modelo B é quem garante a promessa "qualificado".
**O que quer:** medir a qualidade por fonte e por segmento, cortar fonte ruim rápido e realimentar
o score com desfechos reais.
**Jornada:** cadastra/configura fontes (cada uma aprovada pelo DPO antes — 9.9) → roda pipelines de
coleta/dedup/enriquecimento → monitora conversão por fonte → ajusta score/segmentação →
redistribui leads devolvidos/queimados → responde contestações com dados.
**Acessos:** base bruta e quarentena, painel de qualidade por fonte, gestão de pipelines,
histórico de desfechos (anonimizado do que não precisa ver).
**KPIs:** conversão por fonte, % de leads contestados como inválidos, idade média do dado,
cobertura de campos críticos (WhatsApp/e-mail validados).
**Se falhar:** "qualificado" vira marketing sem lastro; a fila enche de lead frio, o closer sai
(9.1), o assinante cancela (9.3) — a falha deste papel aparece primeiro nos KPIs dos outros.

### 9.8 Financeiro / conciliação (interno — crítico no modelo B)

**Quem é:** quem opera o dinheiro: conciliação entre venda fechada, comissão-mãe recebida da
parceira, take rate e repasse ao closer. No MVP é uma pessoa com planilha; na plataforma é um
ledger.
**O que quer:** que cada real de comissão seja rastreável de ponta a ponta e que fechamento fora
da plataforma seja detectável.
**Jornada:** recebe conciliação da parceira → casa venda↔lead↔closer no ledger → aplica take rate
→ executa repasse → trata divergência (venda que a parceira reportou e o closer marcou como
"perdido" = alerta de leakage) → cobra repasses/estornos.
**Acessos:** ledger completo de comissões, integração/conciliação com a parceira, relatórios de
divergência; sem acesso a dados de prospecção que não precisa.
**KPIs:** % de comissões capturadas vs vendas conciliadas (meta ≥ 80% no MVP — §7.5), prazo de
repasse, divergências abertas > 30 dias.
**Se falhar:** o §5.1 deixa de ser risco e vira realidade — receita evapora sem barulho. Este
papel é o teste diário da viabilidade do modelo B.

### 9.9 DPO / compliance (interno)

**Quem é:** encarregado de dados (LGPD, art. 41) — no início um sócio com assessoria jurídica,
nunca "ninguém".
**O que quer:** que cada campo coletado tenha origem e base legal documentadas **antes** de entrar
na base, e que nenhum pedido de titular fique sem resposta no prazo.
**Jornada:** aprova/veta cada nova fonte de dados (com registro da decisão) → mantém o inventário
dados×origem×base legal → atende solicitações de titulares (via 9.5) → audita propagação de
opt-out → responde ANPD/due diligence de clientes enterprise.
**Acessos:** inventário de dados, fila de solicitações de titulares, log de fontes e vetos;
**poder de bloqueio** sobre fonte não aprovada (o pipeline da curadoria não roda sem o aceite).
**KPIs:** 100% das fontes com base legal documentada, prazo médio de resposta ao titular,
achados em auditoria.
**Se falhar:** a curadoria raspa o que converte (a tensão do §8.3 se resolve para o lado errado) e
o passivo cresce silenciosamente até a primeira denúncia ou a primeira due diligence perdida.

### 9.10 Suporte / customer success (interno)

**Quem é:** quem faz onboarding e arbitra conflitos — no marketplace, o papel que mantém a
confiança dos dois lados.
**O que quer:** closers ativados rápido, disputas resolvidas com critério publicado e sinal
antecipado de churn.
**Jornada:** onboarding (closer: homologação e primeiro lead; assinante: primeiro ICP e primeira
lista) → acompanha ativação → arbitra contestações de lead com prazo e critério contratual
("inválido" = telefone inexistente, fora do ICP, duplicado...) → devolve crédito/lead quando
procedente → escala padrões de problema para curadoria e produto.
**Acessos:** visão dos clientes da carteira, fila de contestações com efeito no extrato/cota,
histórico de interações.
**KPIs:** tempo de ativação, % de contestações resolvidas no SLA, NPS por lado do marketplace,
churn precoce.
**Se falhar:** cada disputa mal resolvida ensina o closer a não confiar no extrato e o assinante a
não renovar — e o boca a boca num vertical pequeno faz o resto.

## 10. Perguntas críticas em aberto (responder antes da spec)

1. **Por onde passa o dinheiro da comissão?** (a pergunta que decide o negócio — §5.1)
2. Qual vertical primeiro, e por quê esse? Quem no time conhece o setor por dentro?
3. Quem é o cliente pagante: o closer (paga pela plataforma/leads) ou a empresa dona do produto
   vendido (paga pelo canal)? São GTMs opostos.
4. Qual a base legal LGPD para cada campo entregue (empresa vs pessoa)? Quem responde como DPO?
5. O que acontece com lead não trabalhado/queimado? (SLA, redistribuição, reputação do closer)
6. Exclusividade do lead? (mesmo lead para 2 closers destrói a confiança dos dois lados)
7. Qual a definição operacional de "qualificado" que o closer aceita **por contrato**?
8. Concorrentes diretos no vertical escolhido — quem já entrega lead + fechamento nele?

## 11. Veredito

**GO condicional ao Modelo B vertical** — a dor é real, a margem do modelo de comissão é ordens
de grandeza melhor que vender lista, e o momento de IA favorece qualificação barata. **Condições:**
resolver o choke point da comissão no vertical escolhido (sem isso, NO-GO para o B), fundação de
dados LGPD-limpa (sem scraping de rede social como pilar) e validação manual (§7.4) antes de
construir plataforma. **NO-GO como concebido na conversa** ("motor horizontal para qualquer
segmento, raspando LinkedIn/Instagram/Maps"): é a versão com maior risco legal, menor
defensabilidade e sem mecanismo de captura de receita.

---

*Gerado com a skill `po` (.claude/skills/po/SKILL.md): sondagem do produto de referência ao vivo,
ceticismo sobre o pedido, perguntas antes de solução. Próximo passo natural: responder §10 e, se
GO, escrever a spec (template §4 da skill) e o handoff à skill `arquitetura`.*
