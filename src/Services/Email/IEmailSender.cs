

public interface IEmailSender
{
    Task SendAsync(string subject, string htmlBody);
}