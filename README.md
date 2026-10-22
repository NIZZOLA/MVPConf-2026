# MVP Conf 2026 — Palestras de Márcio Nizzola

Este repositório tem os **slides** e os **códigos de demonstração** das duas palestras que apresentei na **[MVP Conf 2026](https://www.mvpconf.com.br)**.

| # | Palestra | Trilha | Slides |
|---|----------|--------|--------|
| 1 | **Containers, Azure Monitor, App Insights: o que tem a ver com I.A.?** — *Colocar I.A. em produção não é só conhecer LLM!* | Cloud | [PDF](docs/MVPConf-2026-cloud_e_developers.pdf) · [PPTX](docs/MVPConf-2026-cloud_e_developers.pptx) |
| 2 | **Por que só falam em Python para utilizar I.A.? Quero .NET + Agent Framework!** | Developers | [PDF](docs/MVPConf-2026-developers-dotnet-python.pdf) · [PPTX](docs/MVPConf-2026-developers-dotnet-python.pptx) |

---

## Palestra 1 — Containers, Azure Monitor, App Insights: o que tem a ver com I.A.?

> *"Meu agente funciona na minha máquina, mas quem coloca ele em produção? Onde termina o código e começa a arquitetura?"*

Esta palestra mostra o que é preciso para levar um agente de I.A. para produção no Azure. Ela tem como base a arquitetura de referência de chat do Microsoft Foundry, que usa App Service, Container Apps, Foundry Agent Service, Container Registry, Azure AI Search, Managed Identities, Application Insights e Azure Monitor.

### Assuntos abordados

- **Identidade**: Microsoft Entra ID para o usuário e Managed Identity para o agente, com RBAC e o princípio do menor privilégio.
- **Observabilidade**: OpenTelemetry, Application Insights e Azure Monitor para saber qual modelo foi chamado, quantos tokens foram usados, quais tools foram executadas e quanto tempo levou cada etapa.
- **Hosting**: como escolher entre Azure Web App, Azure Container Apps e AKS.
- **Networking**: VNet, Private Endpoints, NSG/Firewall, API Management e WAF.
- **Dados, contexto e memória**: SQL, Cosmos DB, Storage, AI Search, Redis e Service Bus, além do isolamento dos dados por usuário ou tenant.
- **Secrets e configuração**: Managed Identity com Key Vault, sem nenhuma chave no código.
- **Segurança de aplicações de I.A.**: prompt injection, excesso de permissões do agente (*excessive agency*), vazamento de dados (*data exfiltration*), uso indevido de tools (*tool abuse*) e saída do modelo usada sem validação (*insecure output*).
- **Custos e governança**: *"Funciona" não significa "é sustentável"*.
- **Disaster Recovery**: backup, multi-region, RTO/RPO e failover.
- **Azure Hosted Agents**: o código é seu e o runtime é gerenciado pela plataforma. A palestra mostra quando usar, como dividir as responsabilidades com a plataforma e a comparação com o modelo self-hosted.
- **Do VS Code para o Foundry**: como criar e publicar um Hosted Agent com o **Foundry Toolkit** (`Create new Hosted Agent` → `Deploy Hosted Agent`).

### Código da palestra — [`src/cloud`](src/cloud)

| Projeto | Descrição |
|---------|-----------|
| [`sample1-simple-dotnet-hosted`](src/cloud/sample1-simple-dotnet-hosted) | Hosted Agent "hello world" em **C# (.NET 10)** com Agent Framework e protocolo Responses, publicado no Microsoft Foundry. |
| [`mvp-conf-1`](src/cloud/mvp-conf-1) | Hosted Agent básico em **Python** com Agent Framework (`FoundryChatClient` + `ResponsesHostServer`), com suporte a conversas de vários turnos. |

---

## Palestra 2 — Por que só falam em Python para utilizar I.A.? Quero .NET + Agent Framework!

> *"Se você acha que IA é território exclusivo do Python... a própria Microsoft tem uma resposta pra isso — e ela está escrita em C#."*

Esta palestra é para desenvolvedores .NET que sempre encontram labs e exemplos de I.A. escritos em Python. A proposta é mostrar que dá para criar aplicações com agentes sem sair do ecossistema .NET, usando o **Microsoft Agent Framework (MAF)** e o **Microsoft Foundry**.

### Assuntos abordados

- Por que usar I.A. em produção é diferente de rodar um código de demonstração: segurança desde o projeto (*security by design*), bibliotecas confiáveis e cuidado com versões preview.
- I.A. precisa fazer parte de sistemas reais, com APIs, autenticação, banco de dados, filas, observabilidade, CI/CD e containers.
- Onde o C# se destaca: tipagem estática, ecossistema corporativo, ASP.NET Core, async/await e ferramentas de desenvolvimento.
- A evolução das bibliotecas: chamadas HTTP diretas, SDKs OpenAI e Azure OpenAI, Semantic Kernel e agora o **Microsoft Agent Framework**.
- **Um agente não é um chatbot**: ele combina modelo, instruções, tools, memória e contexto, middleware e workflows.
- O agente não substitui a arquitetura. Ele passa a fazer parte dela.

### Demos e código — [`src/developers`](src/developers)

| Demo | Projeto | Descrição |
|------|---------|-----------|
| 1 — O agente mais simples possível (Python × C#) | [`sample1-python-agent`](src/developers/sample1-python-agent) | Agente básico em Python, usado na comparação com C#. O projeto C# [`Sample1-SingleAgent`](src/developers/Sample1-SingleAgent) ainda é só um esqueleto (`Hello, World!`) e não contém o código do agente. |
| 2 — Tool Calling | [`sample2-functions`](src/developers/sample2-functions) | Agente em C# que usa *function tools* (`[Description]`) para chamar sistemas externos, como a consulta de clima pela API Open-Meteo. |
| 3 — Multiagentes | [`sample3-multiagent`](src/developers/sample3-multiagent) | Workflow em grafo com `WorkflowBuilder`, executores *Writer* e *Reviewer*, aresta condicional, resultado estruturado e saída em streaming. |
| 4 — Hosted Agent | [`sample4-dotnet-my-first-agent`](src/developers/sample4-dotnet-my-first-agent) | Agente em C# empacotado em container e publicado como Hosted Agent no Microsoft Foundry. |
| Extra | [`python-marketing-director`](src/developers/python-marketing-director) | Hosted Agent em Python que faz o papel de um diretor de marketing. |

A solution [`MVP-Conf-2026.sln`](src/developers/MVP-Conf-2026.sln) reúne os projetos .NET da trilha Developers.

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) para os exemplos em C#
- Python 3.x para os exemplos em Python
- Uma assinatura do Azure com um projeto no **Microsoft Foundry** e um modelo publicado
- [Azure Developer CLI (`azd`)](https://learn.microsoft.com/azure/developer/azure-developer-cli/) e/ou a extensão **Foundry Toolkit** do VS Code para publicar os Hosted Agents

Cada projeto tem um `README.md` com as instruções para rodar localmente e publicar. Para configurar, use variáveis de ambiente e arquivos `.env` locais e **nunca faça commit de chaves ou segredos**.

## Referências

- [Documentação do Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/)
- [microsoft/agent-framework](https://github.com/microsoft/agent-framework)
- [Agent Framework Samples](https://github.com/microsoft/Agent-Framework-Samples)
- [Blog do Agent Framework](https://devblogs.microsoft.com/agent-framework/)
- [Foundry Agent Service — Pricing](https://azure.microsoft.com/pricing/details/ai-foundry/)

## Autor

**Márcio Rogério Nizzola**: 5x Microsoft MVP (Developer Technologies), Principal Architect na CI&T, professor na Etec de Itu e fundador do Meetup Itu Developers.

- 🌐 [nizzola.com.br](https://www.nizzola.com.br) · [linktr.ee/NIZZOLA](https://linktr.ee/NIZZOLA)
- 💼 [LinkedIn](https://www.linkedin.com/in/nizzola)
- 🐙 [GitHub](https://github.com/nizzola)
- ✍️ [Medium](https://marcionizzola.medium.com): mais de 120 artigos sobre .NET
- ▶️ [YouTube — Code in 8](https://www.youtube.com/@codein8)
