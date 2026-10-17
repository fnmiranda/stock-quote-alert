
public interface IStockQuoteAlert
{
    Task ProcessQuoteAlertAsync( Quote quote, string action);

}