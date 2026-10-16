public interface IBrapiClient
{
    Task<Quote> GetQuoteAsync(string ticker);
}