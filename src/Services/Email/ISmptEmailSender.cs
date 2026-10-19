
namespace stock_quote_alert.src.Services.Email;
public interface ISmtpEmailSender
{
    Task SendAsync(string subject, string htmlBody, CancellationToken cancellationToken = default);
}