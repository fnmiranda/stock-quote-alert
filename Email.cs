using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class Email
{
    public async Task SendEmail(Quote quote, string action)
    {
        try
        {
            
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddUserSecrets<Email>().AddEnvironmentVariables();
            IConfiguration config = builder.Build();

            SmtpSettings smtpSettings = config.GetSection("SmtpSettings").Get<SmtpSettings>() ?? throw new ArgumentNullException("Erro foi encontrado ao carregar as configurações!");

            string pwd = config["SmtpSettings:Password"] ?? throw new ArgumentNullException("A senha não foi encontrada nas configurações ou no User Secrets!");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtpSettings.SenderName, smtpSettings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(smtpSettings.ReceiverEmail));
            message.Subject = "Report";


            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <h2 style='color: #2c3e50;'>Cotação Atualizada: {quote.Symbol} - {action}</h2>
                    <table border='1' cellpadding='8' style='border-collapse: collapse;'>
                        <tr style='background-color: #f8f9fa;'>
                            <th align='left'>Ativo</th>
                            <td>{quote.Symbol}</td>
                        </tr>
                        <tr>
                            <th align='left'>Preço Atual</th>
                            <td><strong>R$ {quote.RegularMarketPrice:F2}</strong></td>
                        </tr>
                        <tr style='background-color: #f8f9fa;'>
                            <th align='left'>Variação</th>
                            <td style='color: {(quote.RegularMarketChangePercent >= 0 ? "green" : "red")};'>
                                {quote.RegularMarketChangePercent}%
                            </td>
                        </tr>
                        <tr>
                            <th align='left'>Moeda</th>
                            <td>{quote.Currency}</td>
                        </tr>
                    </table>
                </div>
            ";
            
            message.Body = bodyBuilder.ToMessageBody();
            Console.WriteLine("Email sendo enviado");

            using var client = new SmtpClient(); 
            await client.ConnectAsync(smtpSettings.Server, smtpSettings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpSettings.SenderEmail, pwd);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            Console.WriteLine("Email enviado");
            
        } catch(Exception e)
        {
            Console.WriteLine($"Erro no envio: {e}");
        }

    }
}

