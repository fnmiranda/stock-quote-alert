

public class StockService: IStockService
{
    public AlertType EvaluateQuote(Quote quote,decimal lastSendPrice, decimal minPrice, decimal maxPrice)
    {
        decimal curretyPrice = quote.RegularMarketPrice;
        decimal minVariationAccept = 0.3m;

        if (lastSendPrice <= 0)
        {
            if (curretyPrice > maxPrice) return AlertType.SELL;
            if (curretyPrice < minPrice) return AlertType.BUY;
            return AlertType.HOLD;
        }
        
        decimal percentVariation = Math.Abs(((curretyPrice/lastSendPrice)-1)*100m);

        if (curretyPrice > maxPrice && percentVariation >= minVariationAccept) return AlertType.SELL;
        if (curretyPrice < minPrice && percentVariation >= minVariationAccept) return AlertType.BUY;
        return AlertType.HOLD;
    }

    public string ConvertAlertType(AlertType alertLabel)
    {
        if (alertLabel == AlertType.BUY) return "Comprar";
        if (alertLabel == AlertType.SELL) return "Vender";
        return "Não variou";
    }
}