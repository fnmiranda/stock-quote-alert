
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using System.Globalization;
class Program
{
    static async Task Main(string[] args)
    {
        string ticker;
        decimal minPrice; 
        decimal maxPrice;

        if (args.Length < 3)
        {
            Console.WriteLine("Uso incorreto!");
            Console.WriteLine("Exemplo correto (Ativo Valor Mínimo Valor Máximo): dotnet run PETR4 56.15 56.25");
            return;
        }

        
        ticker = args[0].ToUpper();
        

        if(!decimal.TryParse(args[1],out minPrice) || !decimal.TryParse(args[2],out maxPrice))
        {
            Console.WriteLine("Valor númerico incorreto. Exemplo de execução correta: dotnet run PETR4 56.15 56.25");
            return;
        }
  
        minPrice = decimal.Parse(args[1].Replace(',', '.'), CultureInfo.InvariantCulture);
        maxPrice = decimal.Parse(args[2].Replace(',', '.'), CultureInfo.InvariantCulture);

        

        Env.Load();
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddUserSecrets<Program>().AddEnvironmentVariables();
        IConfiguration config = builder.Build();
        var smtpSettings = config.GetSection("SmtpSettings").Get<SmtpSettings>() ?? throw new InvalidOperationException("Seção SmtpSettings não configurada.");

        IEmailSender emailSender = new SmtpEmailSender(smtpSettings);
        IStockQuoteEmailComposer emailComposer = new StockQuoteEmailComposer();
        IStockService service = new StockService();

        var client = new BrapiClient(Environment.GetEnvironmentVariable("BRAPI_API_KEY")!);
        var stockQuoteAlert = new StockQuoteAlert(emailSender,emailComposer); 

        try
        {
            var initialQuote = await client.GetQuoteAsync(ticker);

            if (initialQuote == null)
            {
                Console.WriteLine($"Erro: O ativo '{ticker}' não foi encontrado na API da Brapi. Verifique o código e tente novamente.");
                return; 
            }
            Console.WriteLine($"Ativo localizado: {initialQuote.ShortName}");
        }
        catch (BrapiApiException ex)
        {
            Console.WriteLine($" Erro de integração {ex.Message}");
            return;
        }

        while (true)
        {
            try
            {
                var quote = await client.GetQuoteAsync(ticker);

                if(quote != null)
                {
                    string? action = service.EvaluateQuote(quote, minPrice, maxPrice);
                    if (action != null)
                    {
                        await stockQuoteAlert.ProcessQuoteAlertAsync(quote, action);
                        Console.WriteLine($" Alerta: {action} Ativo - {quote.ShortName} - {quote.Symbol} {quote.RegularMarketPrice}");
                    }
                }
                Thread.Sleep(60000); 
            }
            catch(BrapiApiException ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]: Erro de integração na API: {ex.Message}");
            }
            catch(EmailSendException ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]: Erro de E-mail: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]: Erro inexperado: {ex.Message}");
            }
        }
    }
        

        
    
}

