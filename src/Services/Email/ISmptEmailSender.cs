

public interface ISmtpEmailSender
{
    Task SendAsync(string subject, string htmlBody);
}