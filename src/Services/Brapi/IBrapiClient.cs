namespace stock_quote_alert.src.Services.Brapi;

using stock_quote_alert.src.Models;

public interface IBrapiClient
{
    Task<Quote> GetQuoteAsync(string ticker);
}