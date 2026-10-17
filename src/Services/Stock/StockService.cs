

public class StockService: IStockService
{
    public string? EvaluateQuote(Quote quote, decimal minPrice, decimal maxPrice)
    {
        if (quote.RegularMarketPrice > maxPrice) return "Vender";
        if (quote.RegularMarketPrice < minPrice) return "Comprar";
        return null;
    }
}