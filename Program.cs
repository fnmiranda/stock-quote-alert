
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Superpower;
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
            Console.WriteLine("Exemplo correto (Ativo Valor_Mínimo Valor_Máximo): dotnet run PETR4 56.15 56.25");
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

        ISmtpEmailSender emailSender = new SmtpEmailSender(smtpSettings);
        IStockQuoteEmailComposer emailComposer = new StockQuoteEmailComposer();
        IStockService service = new StockService();
        var stockQuoteAlert = new StockQuoteAlert(emailSender,emailComposer);
        
        var brapiSettings = config.GetSection("BrapiSettings").Get<BrapiSettings>();
        if (brapiSettings?.ApiKey == null || string.IsNullOrWhiteSpace(brapiSettings.ApiKey))
        {
            Console.WriteLine("A ApiKey da Brapi não foi informada ou está vazia no .env / appsettings.json.");
            return;
        }

        int delay = 0;
        if (!int.IsEvenInteger(brapiSettings.Delay))
        {
            delay = 60000;
        }
        delay = brapiSettings.Delay;

        if(brapiSettings == null) return;


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
                        Console.WriteLine($" Aguarde: {alertLabel} significativamente o Ativo - {quote.ShortName} - {quote.Currency} {quote.RegularMarketPrice}");
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

