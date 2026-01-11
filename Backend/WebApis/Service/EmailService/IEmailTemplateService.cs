namespace WebApis.Service.EmailService
{
    public interface IEmailTemplateService
    {
        Task<string> RenderAsync(
            string templatePath,
            Dictionary<string, string> tokens
        );
    }
}
