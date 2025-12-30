using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace WebApis.Service.EmailService
{
    
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
         
        public EmailService(IConfiguration configuration) 
        {
            _config = configuration; 
        }
        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            List<string>? cc = null,
            List<string>? bcc = null
            ){
            var host = _config["Smtp:Host"]!;
            var port = int.Parse(_config["Smtp:Port"]!);
            var username = _config["Smtp:UserName"]!;
            var password = _config["Smtp:Password"]!;

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(username));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            if (cc?.Any() == true)
                message.Cc.AddRange(cc.Where(x => !string.IsNullOrWhiteSpace(x))
                                       .Select(MailboxAddress.Parse));

            if (bcc?.Any() == true)
                message.Bcc.AddRange(bcc.Where(x => !string.IsNullOrWhiteSpace(x))
                                         .Select(MailboxAddress.Parse));

            using var client = new SmtpClient();
            client.CheckCertificateRevocation = false;
            var socketOptions = port switch
            {
                465 => SecureSocketOptions.SslOnConnect,
                587 => SecureSocketOptions.StartTls,
                _ => SecureSocketOptions.Auto
            };

            await client.ConnectAsync(host, port, socketOptions);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

    }
}
