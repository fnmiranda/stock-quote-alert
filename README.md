# Stock Quote Alert
## BrapiClient

Cliente simples em C# para consultar cotações de ativos da bolsa brasileira usando a API da [brapi.dev](https://brapi.dev).

### Funcionalidades

- Consulta de cotação por ticker (ex.: `PETR4`, `VALE3`)
- Autenticação via token Bearer
- Timeout de 20 segundos nas requisições
- Desserialização do JSON em objetos tipados

### Requisitos

- .NET 8 ou superior (usa `required` e `System.Text.Json`)
- Token de acesso da brapi.dev (crie sua conta em https://brapi.dev)

### Estrutura

| Classe | Descrição |
|---|---|
| `Quote` | Dados da cotação: símbolo, nome, preço, variação percentual e moeda |
| `QuoteResponse` | Envelope da resposta da API, contendo a lista `Results` |
| `BrapiClient` | Cliente HTTP que realiza a chamada e retorna a cotação |

### Como usar

````csharp
var client = new BrapiClient("SEU_TOKEN_AQUI");

var quote = await client.GetQuoteAsync("PETR4");

if (quote != null)
{
    Console.WriteLine($"{quote.Symbol} - {quote.ShortName}");
    Console.WriteLine($"Preço: {quote.RegularMarketPrice} {quote.Currency}");
    Console.WriteLine($"Variação: {quote.RegularMarketChangePercent}%");
}
````

### Como funciona

1. O `BrapiClient` é criado com o token, que é enviado no header `Authorization: Bearer <token>`.
2. `GetQuoteAsync` faz um `GET` em `https://brapi.dev/api/quote/{ticker}`.
3. O JSON retornado é desserializado (sem diferenciar maiúsculas/minúsculas nos nomes das propriedades) em `QuoteResponse`.
4. O primeiro item de `Results` é retornado como `Quote`.

### Referência

- Documentação da API: https://brapi.dev/docs
````
````