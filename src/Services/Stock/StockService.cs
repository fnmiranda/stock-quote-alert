

public class StockService: IStockService
{
    public AlertType EvaluateQuote(Quote quote,decimal lastSendPrice, decimal minPrice, decimal maxPrice)
    {
        decimal curretyPrice = quote.RegularMarketPrice;

        if (lastSendPrice <= 0)
        {
            if (curretyPrice > maxPrice) return AlertType.SELL;
            if (curretyPrice < minPrice) return AlertType.BUY;
            return AlertType.HOLD;
        }
        
        decimal variation = Math.Abs((lastSendPrice - curretyPrice)*1m);

        if (curretyPrice > maxPrice && variation > 0) return AlertType.SELL;
        if (curretyPrice < minPrice && variation > 0) return AlertType.BUY;
        return AlertType.HOLD;
    }

    public string ConvertAlertType(AlertType alertLabel)
    {
        if (alertLabel == AlertType.BUY) return "Comprar";
        if (alertLabel == AlertType.SELL) return "Vender";
        return "Não variou";
    }
}