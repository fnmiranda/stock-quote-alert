using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public class SmtpEmailSender : ISmtpEmailSender
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
            
            if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
            {
                try{
                    await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
                }
                catch (AuthenticationException)
                {
                    throw new EmailSendException("Falha na autenticação SMTP: Usuário ou Senha de App incorretos.");
                }
                catch (SmtpCommandException ex)
                {
                    throw new EmailSendException($"Servidor recusou a autenticação ({ex.StatusCode}): {ex.Message}");
                }
                catch (SmtpProtocolException ex)
                {
                    if (ex.Message.Contains("5.7.8") || ex.Message.Contains("BadCredentials") || ex.Message.Contains("not accepted"))
                    {
                        throw new EmailSendException("Falha na autenticação SMTP: O e-mail ou a Senha de App informados são inválidos.");
                    }

                    throw new EmailSendException("A conexão com o servidor SMTP foi interrompida durante o login.");
                }
            }
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

        }
        catch (EmailSendException)
        {
            throw;
        }
        
        catch (SmtpCommandException ex)
        {
            throw new EmailSendException($"Erro no servidor SMTP ({ex.StatusCode}): {ex.Message}");
        } 
        catch(Exception ex)
        {
            throw new EmailSendException($"Falha inesperada ao enviar e-mail: {ex.Message}");
        }
    }
}