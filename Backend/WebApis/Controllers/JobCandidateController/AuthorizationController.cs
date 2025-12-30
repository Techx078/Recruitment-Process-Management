using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApis.Service.AuthorizationService;

namespace WebApis.Controllers.JobCandidateController
{
    [ApiController]
    [Route("api/authorization")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthorizationController(
           IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("hr-level/{jobOpeningId}")]
        [Authorize(Roles = "Interviewer,Recruiter,Admin")]
        public async Task<IActionResult> ValidateHrInterviewer(int jobOpeningId)
        {
            await _authService.ValidateHrLevelAccessAsync(jobOpeningId, User);
            return Ok(new { authorized = true });
        }
    }

}
