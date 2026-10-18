# Stock Quote Alert 📈✉️

Application em linha de comando (CLI) desenvolvida em **C# / .NET** para monitoramento contínuo de cotações do mercado financeiro e disparo automatizado de alertas formatados por e-mail quando alvos de compra ou venda são atingidos.

---

## 🚀 Funcionalidades

- **Monitoramento em Tempo Real:** Consulta a API da [Brapi](https://brapi.dev/) em intervalos(Delay) regulares configuráveis no appsettings.json.
- **Notificações por E-mail:** Disparo de e-mails em HTML responsivo e estilizado com resumo da cotação e recomendação de ação.
- **Gestão Segura de Credenciais:** Separação estrita entre código e segredos utilizando **User Secrets** do .NET e variáveis de ambiente (evitando vazamento de senhas em repositórios Git).
- **Tratamento de Erros e Resiliência:** Validação de tickers inexistentes e exceções customizadas (`BrapiApiException`).

---
## 💻 Execução
- Exemplo de execução o dotnet:
`
   dotnet run PETR3 56.15 56.25
`

---
## ⚙️ Funcionamento das Configurações (`appsettings.json` vs `.env`)

O projeto utiliza o sistema de hierarquia de configurações do .NET, onde **as variáveis de ambiente do arquivo `.env` sobrescrevem as chaves do `appsettings.json`**.

### Como funciona a integração:

1. **`appsettings.json` (Estrutura e Padrões):**
   Define a estrutura das configurações e armazena dados não sensíveis de infraestrutura (como o endereço do servidor `smtp.gmail.com` e a porta `587`).

2. **`.env` / Variáveis de Ambiente (Segredos e Credenciais):**
   Armazena credenciais sensíveis e dados específicos de ambiente. Para mapear propriedades aninhadas do JSON via variáveis de ambiente, utilize o separador de **duplo sublinhado (`__`)**:

   | Variável no `.env` | Chave Equivalente no `appsettings.json` |
   | :--- | :--- |
   | `SmtpSettings__SenderEmail` | `SmtpSettings:SenderEmail` |
   | `SmtpSettings__Username` | `SmtpSettings:Username` |
   | `SmtpSettings__Password` | `SmtpSettings:Password` |
   | `BrapiSettings__Apikey` | `BrapiSettings:ApiKey` |

### Exemplo de `.env` para Desenvolvimento Local:
   Obs.: Lembrando que a SmtpSettings__Password não é a senha e sim o codigo de 16 digitos disponibilizado pelo google. Para sabe **[clique aqui](https://support.google.com/mail/answer/185833)**

```env
BrapiSettings__Apikey="seu_token_aqui"

SmtpSettings__SenderEmail="seu_email@gmail.com"
SmtpSettings__Username="seu_email@gmail.com"
SmtpSettings__ReceiverEmail="destinatario@email.com"
SmtpSettings__Password="sua_senha_de_app"
```
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

## 🎯 Decisão de Arquitetura: Dependências Modulares vs. Host Genérico

Considerando o escopo enxuto e a natureza focada da aplicação (CLI), optou-se por **importar cada pacote do ecossistema `.Extensions` de forma individual e pontual** (como `Microsoft.Extensions.Configuration` e `Microsoft.Extensions.Configuration.UserSecrets`), em vez de incluir a infraestrutura completa do `Microsoft.Extensions.Hosting`.

### Principais Motivações:

- **Menor Overhead (Footprint Leve):** Evita incluir abstrações de ciclo de vida de Host e dependências não utilizadas para um utilitário de terminal direto.
- **Controle e Transparência:** Cada biblioteca adicionada ao `.csproj` possui uma responsabilidade única e explícita no projeto.
- **Simplicidade de Execução:** Mantém a inicialização e a leitura de configurações diretas, reduzindo o tempo de boot e a complexidade do código de setup.

---

## 🎨 Template HTML dos E-mails

O layout do e-mail de notificação foi **gerado com o auxílio de Inteligência Artificial**, permitindo a criação de uma estrutura HTML responsiva, estilizada e compatível com os principais leitores de e-mail, sem misturar a lógica de design com a lógica de envio do C#.

---



## 📁 Arquitetura e Estrutura do Projeto

O projeto adota uma organização baseada no princípio de **Separação de Responsabilidades (SoC)** e **Clean Code**, isolando configurações, modelos, exceções e serviços (**desenvolvido com o auxílio de Inteligência Artificial**):

```text
src/
├── Configuration/
│   ├── SmtpSettings.cs               # Mapeamento fortemente tipado das configurações de e-mail
│   └── BrapiSettings.cs              # Mapeamento fortemente tipado das configurações da API do Brapi
├── Exceptions/
│   └── Exceptions.cs                 # Exceção de domínio para falhas na API de cotações e do Email SMTP
├── Models/
│   ├── Quote.cs                      # Modelo de dados da cotação
│   ├── QuoteResponse.cs              # DTO para desserialização da resposta JSON
│   └── AlertType.cs                  # Enum com os valores BUY, SELL, e HOLD
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