namespace stock_quote_alert.src.Services.Stock;

using stock_quote_alert.src.Models;

public class StockService: IStockService
{
    public AlertType EvaluateQuote(Quote quote,decimal lastSendPrice, decimal minPrice, decimal maxPrice)
    {
        decimal curretyPrice = quote.RegularMarketPrice;
        decimal sensibility = 3m;

        if (lastSendPrice <= 0)
        {
            if (curretyPrice > maxPrice) return AlertType.SELL;
            if (curretyPrice < minPrice) return AlertType.BUY;
            return AlertType.HOLD;
        }
        
        decimal variation = Math.Abs((curretyPrice/lastSendPrice - 1)*100m);

        if (curretyPrice > maxPrice && variation > sensibility) return AlertType.SELL;
        if (curretyPrice < minPrice && variation > sensibility) return AlertType.BUY;
        return AlertType.HOLD;
    }

    public string ConvertAlertType(AlertType alertLabel)
    {
        if (alertLabel == AlertType.BUY) return "Comprar";
        if (alertLabel == AlertType.SELL) return "Vender";
        return "Não variou";
    }
}