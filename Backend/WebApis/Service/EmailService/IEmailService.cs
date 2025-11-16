namespace WebApis.Service.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toEmail, 
            string subject, 
            string body,
            List<String> cc = null,
            List<String> bcc = null
            );

    }
}
