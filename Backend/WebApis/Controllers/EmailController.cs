using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApis.Service.EmailService;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto request )
    {
        await _emailService.SendEmailAsync(
            request.ToEmail,
            request.Subject,
            request.Body,
            request.Cc ,
            request.Bcc 
        );

        return Ok("Email Sent");
    }
}
