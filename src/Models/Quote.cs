namespace stock_quote_alert.src.Models;

public class Quote
{
    public required string Symbol { get; set; }
    public required string ShortName { get; set; }
    public required decimal RegularMarketPrice { get; set; }
    public required decimal RegularMarketChangePercent { get; set; }
    public required string Currency { get; set; }
}