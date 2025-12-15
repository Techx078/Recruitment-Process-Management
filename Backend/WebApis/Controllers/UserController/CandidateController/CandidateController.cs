using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Repository.CandidateRepository;
using WebApis.Service.ErrroHandlingService;
using WebApis.Service.ValidationService;

namespace WebApis.Controllers.UserController.CandidateController
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly ICommonValidator<UpdateCandidateDto> _updateCandidateValidator;
        public CandidateController(ICandidateRepository candidateRepository, ICommonValidator<UpdateCandidateDto> updateCandidateValidator) {
            _candidateRepository = candidateRepository;
            _updateCandidateValidator = updateCandidateValidator;
        }
        [HttpGet("{userId}")]
        [Authorize(Roles = "Recruiter,Interviewer,Admin,Reviewer,Candidate")]
        public async Task<IActionResult> GetCandidateDetailsByUserId(int userId)
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var userRoleClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            if (userRoleClaim == "Candidate" && loggedInUserId != userId)
                throw new AppException("You are unauthorized candidate", 403);

            var candidate = await _candidateRepository.GetCandidateDetailsByUserId(userId);

            
            if (candidate == null)
                throw new KeyNotFoundException("Candidate not found");

            return Ok(candidate);
        }

        [HttpGet("jobOpening/{userId}")]
        [Authorize(Roles = "Recruiter,Interviewer,Admin,Reviewer,Candidate")]
        public async Task<IActionResult> GetCnadidateJobOpeningDetailsByUserId(int userId)
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var userRoleClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            if (userRoleClaim == "Candidate" && loggedInUserId != userId)
                throw new AppException("You are not authorized", 403);

            var jobOpenings =
                await _candidateRepository.GetCnadidateJobOpeningDetailsByUserId(userId);

            if (jobOpenings == null)
                throw new KeyNotFoundException("Job openings not found");

            return Ok(jobOpenings);
        }

        [HttpPut("update/{UserId}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> UpdateCandidateDetails(int UserId ,UpdateCandidateDto dto )
        {
            var validationResult = await _updateCandidateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    errors = validationResult.Errors
                });
            }
            //check for valid candidate
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException();
            }

            int loggedInUserId = int.Parse(userIdClaim);
            if (loggedInUserId != UserId)
            {
                throw new AppException("You are not authorized", 403);
            }
           
            var candidate = await _candidateRepository.GetCandidateWithUserAsync(UserId);

            if (candidate == null)
                throw new KeyNotFoundException("Candidate not found");

            await _candidateRepository.UpdateCandidateAsync(candidate, dto);
            return Ok(new
            {
                message = "Candidate updated successfully.",
                candidateId = candidate.Id
            });
        }
    }
}
