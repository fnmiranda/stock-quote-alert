
namespace stock_quote_alert.src.Services.Stock;

using stock_quote_alert.src.Models;
public interface IStockService
{
    public AlertType EvaluateQuote(Quote quote, decimal lastSendPrice, decimal minPrice, decimal maxPrice);

    public string ConvertAlertType(AlertType alertLabel);
}