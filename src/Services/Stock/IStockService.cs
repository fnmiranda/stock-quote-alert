

public interface IStockService
{
    public AlertType EvaluateQuote(Quote quote, decimal lastSendPrice, decimal minPrice, decimal maxPrice);

    public string ConvertAlertType(AlertType alertLabel);
}