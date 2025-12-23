using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Repository;
using WebApis.Service.ErrorHandlingService;
using WebApis.Service.ErrroHandlingService;

namespace WebApis.Controllers.UserController.RecruiterController
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecruiterController : Controller
    {
        private readonly ICommonRepository<Recruiter> _recruiterRepository;
        private readonly ICommonRepository<JobOpening> _jobOpeningRepository;

        public RecruiterController(
            ICommonRepository<Recruiter> recruiterRepository,
            ICommonRepository<JobOpening> jobOpeningRepository)
        {
            _recruiterRepository = recruiterRepository;
            _jobOpeningRepository = jobOpeningRepository;
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> GetRecruiterById(int userId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException();

            var loggedInUserId = int.Parse(userIdClaim);

            if (userRoleClaim == "Recruiter" && loggedInUserId != userId)
                throw new AppException(
                    "You don’t have permission to view this recruiter.",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            var recruiter = await _recruiterRepository.GetByFilterAsync(r => r.UserId == userId);
            if (recruiter == null)
                throw new AppException(
                    "Recruiter not found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            var createdJobOpenings = await _jobOpeningRepository.GetAllByFilterAsync(
                j => j.CreatedById == recruiter.Id,
                j => new AssignedJobOpeningDto
                {
                    JobOpeningId = j.Id,
                    Title = j.Title,
                    Status = j.Status,
                    Department = j.Department,
                    JobType = j.JobType,
                    CreatedAt = j.CreatedAt,
                    CandidateCount = j.JobCandidates.Count,
                    MinDomainExperience = j.minDomainExperience,
                    Domain = j.Domain
                });

            var recruiterDetails = await _recruiterRepository.GetByFilterAsync(
                r => r.Id == recruiter.Id,
                r => new ReviewerInterviewerDetailsDto
                {
                    Id = r.Id,
                    Department = r.Department,
                    User = new UserDto
                    {
                        Id = r.User.Id,
                        FullName = r.User.FullName,
                        Email = r.User.Email,
                        PhoneNumber = r.User.PhoneNumber,
                        Domain = r.User.Domain,
                        DomainExperienceYears = r.User.DomainExperienceYears,
                    },
                    AssignedJobOpenings = createdJobOpenings
                }
            );

            return Ok(recruiterDetails);
        }
    }
}
