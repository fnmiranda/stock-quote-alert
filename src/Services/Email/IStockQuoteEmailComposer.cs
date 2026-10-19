namespace stock_quote_alert.src.Services.Email;
using stock_quote_alert.src.Models;

public interface IStockQuoteEmailComposer
{
    (string Subject, string HtmlBody) BuildStockAlertEmail(Quote quote, string action);
}