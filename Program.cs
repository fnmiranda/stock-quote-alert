namespace stock_quote_alert;

using DotNetEnv;
using Microsoft.Extensions.Configuration;
using System.Globalization;

using stock_quote_alert.src.Models;
using stock_quote_alert.src.Services.Stock;
using stock_quote_alert.src.Services.Email;
using stock_quote_alert.src.Services.Brapi;
using stock_quote_alert.src.Configuration;
using stock_quote_alert.src.Exceptions;
class Program
{
    static async Task Main(string[] args)
    {
        string ticker;

        if (args.Length < 3)
        {
            Console.WriteLine("Uso incorreto!");
            Console.WriteLine("Exemplo correto (Ativo Valor_Mínimo Valor_Máximo): dotnet run PETR4 56.15 56.25");
            return;
        }
        ticker = args[0].ToUpper();

        if(!decimal.TryParse(args[1],out decimal minPrice) || !decimal.TryParse(args[2],out decimal maxPrice))
        {
            Console.WriteLine("Valor númerico incorreto. Exemplo de execução correta: dotnet run PETR4 56.15 56.25");
            return;
        }
        minPrice = decimal.Parse(args[1].Replace(',', '.'), CultureInfo.InvariantCulture);
        maxPrice = decimal.Parse(args[2].Replace(',', '.'), CultureInfo.InvariantCulture); 



        Env.Load();
        IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddUserSecrets<Program>().AddEnvironmentVariables().Build();

        var smtpSettings = config.GetSection("SmtpSettings").Get<SmtpSettings>() ?? throw new InvalidOperationException("Seção SmtpSettings não configurada.");

        ISmtpEmailSender emailSender = new SmtpEmailSender(smtpSettings);
        IStockQuoteEmailComposer emailComposer = new StockQuoteEmailComposer();
        IStockService service = new StockService();
        IStockQuoteAlertService stockQuoteAlert = new StockQuoteAlertService(emailSender,emailComposer);
        

        var brapiSettings = config.GetSection("BrapiSettings").Get<BrapiSettings>();
        if(brapiSettings == null) return;

        if (brapiSettings?.ApiKey == null || string.IsNullOrWhiteSpace(brapiSettings.ApiKey))
        {
            Console.WriteLine("A ApiKey da Brapi não foi informada ou está vazia no .env / appsettings.json.");
            return;
        }

        int delay =  int.TryParse(brapiSettings.Delay, out int parsedDaley) ? parsedDaley : 30000;
        var client = new BrapiClient(brapiSettings);
        
        decimal lastSendPrice = 0m;
        while (true)
        {
            try
            {
                var quote = await client.GetQuoteAsync(ticker);

                if(quote != null)
                {
                    AlertType alert = service.EvaluateQuote(quote, lastSendPrice, minPrice, maxPrice);
                    string alertLabel = service.ConvertAlertType(alert);
                    if (alert != AlertType.HOLD)
                    {
                        await stockQuoteAlert.ProcessQuoteAlertAsync(quote, alertLabel);
                        Console.WriteLine($" Alerta: {alertLabel} Ativo - {quote.ShortName} - {quote.Currency} {quote.RegularMarketPrice}");
                        lastSendPrice = quote.RegularMarketPrice;
                    }
                    else
                    {
                        Console.WriteLine($" Aguarde: o Ativo {alertLabel} - {quote.ShortName} - {quote.Currency} {quote.RegularMarketPrice}");
                    }
                }
                Thread.Sleep(delay); 
            }
            catch(BrapiApiException ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]: Erro de integração na API: {ex.Message}");
                return;
            }
            catch(EmailSendException ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]: Erro de E-mail: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]: Erro inexperado: {ex.Message}");
                return;
            }
        }
    }
        

        
    
}

