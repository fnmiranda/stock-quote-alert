using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;

    public SmtpEmailSender(SmtpSettings smtpSettings)
    {
        _settings = smtpSettings;
    }

    public async Task SendAsync(string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(_settings.ReceiverEmail));
            message.Subject = subject;


            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient(); 
            var security = _settings.Port == 465  ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
            await client.ConnectAsync(_settings.Server, _settings.Port, security);
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
        } catch(Exception ex)
        {
            Console.WriteLine($"Erro no envio do Email: {ex.Message}");
        }
    }
}