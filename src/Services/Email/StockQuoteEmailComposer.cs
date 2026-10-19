
namespace stock_quote_alert.src.Services.Email;

using stock_quote_alert.src.Models;
public class StockQuoteEmailComposer: IStockQuoteEmailComposer
{

    public  (string Subject, string HtmlBody) BuildStockAlertEmail(Quote quote, string action)
    {
        string subject = $"Alerta {quote.Symbol}: {action}";

        string htmlBody = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2 style='color: #2c3e50;'>Cotação Atualizada: {quote.Symbol} - {action}</h2>
                <table border='1' cellpadding='8' style='border-collapse: collapse;'>
                    <tr style='background-color: #f8f9fa;'>
                        <th align='left'>Ativo</th>
                        <td>{quote.Symbol}</td>
                    </tr>
                    <tr>
                        <th align='left'>Preço Atual</th>
                        <td><strong> {quote.RegularMarketPrice:F2}</strong></td>
                    </tr>
                    <tr style='background-color: #f8f9fa;'>
                        <th align='left'>Variação</th>
                        <td style='color: {(quote.RegularMarketChangePercent >= 0 ? "green" : "red")};'>
                            {quote.RegularMarketChangePercent}%
                        </td>
                    </tr>
                    <tr>
                        <th align='left'>Moeda</th>
                        <td>{quote.Currency}</td>
                    </tr>
                </table>
            </div>";
        return (subject, htmlBody);
    }
}