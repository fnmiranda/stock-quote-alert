using System;
using System.Threading;

class Program
{
    public static void LogController(dynamic quote, string sale)
    {
        Console.WriteLine($"{quote.Symbol} - {sale}");
        Console.WriteLine($"RegularMarketPrice: R$ {quote.RegularMarketPrice:F2}");
        Console.WriteLine($"RegularMarketChangePercent: {quote.RegularMarketChangePercent}%");
        Console.WriteLine($"Currency: {quote.Currency}");
    }
    static async Task Main()
    {
        int w = 10;
        while (w > 0)
        {

            var email = new EmailController();

            var client = new BrapiClient(Environment.GetEnvironmentVariable("BRAPI_API_KEY")!);
            string ticker = "PETR4";
            var quote = await client.GetQuoteAsync(ticker);

            if(quote != null)
            {

                string sale = "";

                if (quote.RegularMarketPrice>56.25m)
                {
                    sale = "Vender";
                    await email.SendEmail(quote,sale);
                    LogController(quote, sale);
                }
                else if (quote.RegularMarketPrice<56.15m)
                {
                    sale = "Comprar";
                    await email.SendEmail(quote,sale);
                    LogController(quote, sale);

                }else
                {
                    LogController(quote, "Nada");
                }
                
                
            }
            Thread.Sleep(60000); 
            w--;
        }
        

        
    }
}

