

public interface IStockService
{
    public string? EvaluateQuote(Quote quote, decimal minPrice, decimal maxPrice);
}