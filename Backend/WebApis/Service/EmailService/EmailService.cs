using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
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
            string body , 
            List<String> cc = null , 
            List<String> bcc= null 
        ){
            if (string.IsNullOrEmpty(toEmail))
            {
                throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(toEmail));
            }
            if (string.IsNullOrEmpty(subject)) {
                throw new ArgumentException("Email subject cannot be null or empty.", nameof(subject));
            }
            if (string.IsNullOrEmpty(body)) {
                throw new ArgumentException("Email body cannot be null or empty.", nameof(body));
            }

            //cofiguration
            var host = _config["Smtp:Host"] ?? throw new Exception("SMTP Host is missing in configuration.");
            var portString = _config["Smtp:Port"] ?? throw new Exception("SMTP Port is missing in configuration.");
            var username = _config["Smtp:UserName"] ?? throw new Exception("SMTP Username is missing in configuration.");
            var password = _config["Smtp:Password"] ?? throw new Exception("SMTP Password is missing in configuration.");

            if (!int.TryParse(portString, out int port))
                throw new Exception("SMTP Port must be a valid number.");

            bool enableSSL = bool.TryParse(_config["Smtp:EnableSSL"], out bool ssl) ? ssl : true;

            // Create the email message
            var message = new MimeMessage();
            // Set the sender's address
            message.From.Add(new MailboxAddress(username,username));
            // Set the recipient's address
            message.To.Add(new MailboxAddress( "",toEmail));
            // Set the subject
            message.Subject = subject;

            //add cc and bcc if any
            if( cc != null && cc.Any())
            {
                foreach (var ccEmail in cc.Where( e => !String.IsNullOrEmpty(e) ) )
                {
                    message.Cc.Add(new MailboxAddress("", ccEmail.Trim()));
                }
            }
            if( bcc != null && bcc.Any())
            {
                foreach (var bccEmail in bcc.Where( e => !String.IsNullOrEmpty(e) ) )
                {
                    message.Bcc.Add(new MailboxAddress("", bccEmail.Trim()));
                }
            }
            // Set the body
            message.Body = new BodyBuilder { HtmlBody = body
            }.ToMessageBody();

            try
            {
                // Send the email
                // Create a new SMTP client
                using var client = new SmtpClient();

                try
                {
                    // Connect to the SMTP server
                    await client.ConnectAsync(host, port, enableSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect to SMTP server: {ex.Message}");
                    throw ex;
                }
                try
                {
                    // Authenticate with the SMTP server
                    await client.AuthenticateAsync(username, password);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SMTP authentication failed: {ex.Message}");
                    throw ex;
                }
                try
                {
                    // Send the email
                    await client.SendAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                    throw ex;
                }

                // Disconnect from the SMTP server
                await client.DisconnectAsync(true);
                Console.WriteLine($"Email sent to {toEmail} with subject '{subject}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while sending email: {ex.Message}");
                throw ex;
            }
        }
    }
}
