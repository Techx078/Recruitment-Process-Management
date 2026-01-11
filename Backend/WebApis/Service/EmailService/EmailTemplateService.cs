using System.Text;

namespace WebApis.Service.EmailService
{
    public class EmailTemplateService:IEmailTemplateService
    {
       
            private readonly IWebHostEnvironment _env;

            public EmailTemplateService(IWebHostEnvironment env)
            {
                _env = env;
            }

            public async Task<string> RenderAsync(
                string templatePath,
                Dictionary<string, string> tokens)
            {
                var fullPath = Path.Combine(
                    _env.ContentRootPath,
                    "EmailTemplates",
                    templatePath
                );

                if (!File.Exists(fullPath))
                    throw new FileNotFoundException($"Email template not found: {templatePath}");

                var html = await File.ReadAllTextAsync(fullPath, Encoding.UTF8);

                foreach (var token in tokens)
                {
                    html = html.Replace(
                        $"{{{{{token.Key}}}}}",
                        token.Value ?? string.Empty
                    );
                }

                return html;
            }
    }
}


