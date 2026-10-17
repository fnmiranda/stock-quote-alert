# Stock Quote Alert 📈✉️

Application em linha de comando (CLI) desenvolvida em **C# / .NET** para monitoramento contínuo de cotações do mercado financeiro e disparo automatizado de alertas formatados por e-mail quando alvos de compra ou venda são atingidos.

---

## 🚀 Funcionalidades

- **Monitoramento em Tempo Real:** Consulta a API da [Brapi](https://brapi.dev/) em intervalos regulares configuráveis.
- **Notificações por E-mail:** Disparo de e-mails em HTML responsivo e estilizado com resumo da cotação e recomendação de ação.
- **Gestão Segura de Credenciais:** Separação estrita entre código e segredos utilizando **User Secrets** do .NET e variáveis de ambiente (evitando vazamento de senhas em repositórios Git).
- **Tratamento de Erros e Resiliência:** Validação de tickers inexistentes e exceções customizadas (`BrapiApiException`).

---

## 🛠️ Tecnologias e Bibliotecas

- **[C# / .NET 8.0+](https://dotnet.microsoft.com/)** — Plataforma de desenvolvimento principal.
- **[MailKit](https://github.com/jstedfast/MailKit) & [MimeKit](https://github.com/jstedfast/MimeKit)** — Envio de e-mails via SMTP e construção do corpo das mensagens em HTML.
- **`Microsoft.Extensions.Configuration`** — Provedor unificado de configurações (`appsettings.json`, *User Secrets*, Variáveis de Ambiente).
- **`Microsoft.Extensions.DependencyInjection`** — Gerenciamento do ciclo de vida dos serviços e desacoplamento via interfaces.
- **`Microsoft.Extensions.Configuration.Binder`**  Mapeamento automático de seções JSON para classes C# fortemente tipadas (`.Get<T>()`). 
- **`Microsoft.Extensions.Configuration.UserSecrets`**  Armazenamento e leitura segura de credenciais locais em ambiente de desenvolvimento. 
- **`Microsoft.Extensions.Configuration.EnvironmentVariables`**  Suporte para leitura de variáveis de ambiente do sistema operacional. 
- **`DotNetEnv`**  Leitura de variáveis de ambiente a partir de arquivos `.env` locais. |
- **`System.Security.Cryptography.Pkcs`**  Correção explícita de segurança para mitigação de vulnerabilidades transitivas. 

---

## 📁 Arquitetura e Estrutura do Projeto

O código é organizado em camadas claras seguindo a separação de responsabilidades (SoC):

```text
src/
├── Configuration/
│   └── SmtpSettings.cs               # Mapeamento fortemente tipado das configurações de e-mail
├── Exceptions/
│   └── BrapiApiException.cs          # Exceção de domínio para falhas na API de cotações
├── Models/
│   ├── Quote.cs                      # Modelo de dados da cotação
│   └── QuoteResponse.cs              # DTO para desserialização da resposta JSON
└── Services/
    ├── Brapi/
    │   ├── IBrapiClient.cs           # Interface do cliente HTTP
    │   └── BrapiClient.cs            # Implementação do consumo da API Brapi
    ├── Email/
    │   ├── IEmailSender.cs           # Interface de envio de e-mails
    │   ├── SmtpEmailSender.cs        # Implementação SMTP usando MailKit
    │   ├── IStockQuoteEmailComposer.cs # Interface para montagem do HTML do e-mail
    │   └── StockQuoteEmailComposer.cs  # Gerador dos templates de e-mail (Stateless)
    └── Stock/
        ├── IStockQuoteAlert.cs       # Interface do orquestrador de regras de geração da mensagem do email
        ├── StockQuoteAlert.cs        # Orquestrador de regras de geração da mensagem do email
        ├── IStockService.cs          # Interface da classe StockService
        └── StockService.cs           # Avaliador das regras de negócio (Comprar/Vender)