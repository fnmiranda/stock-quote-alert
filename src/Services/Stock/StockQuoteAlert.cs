
public class StockQuoteAlert : IStockQuoteAlert
{

    private readonly ISmtpEmailSender _emailSender;
    private readonly IStockQuoteEmailComposer _emailComposer;
    public StockQuoteAlert(ISmtpEmailSender emailSender, IStockQuoteEmailComposer emailComposer)
    {
        _emailSender = emailSender;
        _emailComposer = emailComposer;
    }

    public async Task ProcessQuoteAlertAsync( Quote quote, string action)
    {
        var (subject, htmlBody) = _emailComposer.BuildStockAlertEmail(quote, action);

        await _emailSender.SendAsync(subject, htmlBody);
    }
}