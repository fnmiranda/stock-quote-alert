

public interface IStockQuoteEmailComposer
{
    (string Subject, string HtmlBody) BuildStockAlertEmail(Quote quote, string action);
}