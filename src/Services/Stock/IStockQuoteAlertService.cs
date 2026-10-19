namespace stock_quote_alert.src.Services.Stock;

using stock_quote_alert.src.Models;

public interface IStockQuoteAlertService
{
    Task ProcessQuoteAlertAsync( Quote quote, string action);

}