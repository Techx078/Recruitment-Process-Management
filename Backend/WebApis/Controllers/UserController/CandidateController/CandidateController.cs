using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Repository.CandidateRepository;
using WebApis.Service.ErrorHandlingService;
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
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException();

            var loggedInUserId = int.Parse(userIdClaim);

            if (userRoleClaim == "Candidate" && loggedInUserId != userId)
                throw new AppException(
                    "You don’t have permission to view this candidate.",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            var candidate = await _candidateRepository.GetCandidateDetailsByUserId(userId);

            if (candidate == null)
                throw new AppException(
                    "Candidate not found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            return Ok(candidate);
        }


        [HttpGet("jobOpening/{userId}")]
        [Authorize(Roles = "Recruiter,Interviewer,Admin,Reviewer,Candidate")]
        public async Task<IActionResult> GetCnadidateJobOpeningDetailsByUserId(int userId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException();

            var loggedInUserId = int.Parse(userIdClaim);

            if (userRoleClaim == "Candidate" && loggedInUserId != userId)
                throw new AppException(
                    "You don’t have permission to view these job openings.",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            var jobOpenings =
                await _candidateRepository.GetCnadidateJobOpeningDetailsByUserId(userId);

            return Ok(jobOpenings);
        }


        [HttpPut("update/{userId}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> UpdateCandidateDetails(int userId, [FromBody]UpdateCandidateDto dto)
        {
            var validationResult = await _updateCandidateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new AppException(
                    "Please fill all required fields correctly.",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest,
                    validationResult.Errors
                );
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException();

            var loggedInUserId = int.Parse(userIdClaim);
            if (loggedInUserId != userId)
            {
                throw new AppException(
                    "You don’t have permission to update this candidate.",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );
            }

            var candidate = await _candidateRepository.GetCandidateWithUserAsync(userId);
            if (candidate == null)
            {
                throw new AppException(
                    "Candidate not found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );
            }

            await _candidateRepository.UpdateCandidateAsync(candidate, dto);

            return Ok(new
            {
                message = "Candidate updated successfully.",
                candidateId = candidate.Id
            });
        }

    }
}
