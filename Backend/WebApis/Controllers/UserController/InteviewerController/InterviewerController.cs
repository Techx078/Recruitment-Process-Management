using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Repository;
using WebApis.Service.ErrorHandlingService;
using WebApis.Service.ErrroHandlingService;

namespace WebApis.Controllers.UserController.InteviewerController
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewerController : Controller
    {
        private readonly ICommonRepository<Interviewer> _interviewerRepository;

        public InterviewerController(ICommonRepository<Interviewer> interviewerRepository)
        {
            _interviewerRepository = interviewerRepository;
        }

        [HttpGet("All")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> GetAllInterviewers()
        {
            var interviewerList = await _interviewerRepository.GetAllByFilterAsync(
                i => true,
                i => new ReviewerInterviewerDetailsDto
                {
                    Id = i.Id,
                    Department = i.Department,
                    User = new UserDto
                    {
                        Id = i.User.Id,
                        FullName = i.User.FullName,
                        Email = i.User.Email,
                        PhoneNumber = i.User.PhoneNumber,
                        Domain = i.User.Domain,
                        DomainExperienceYears = i.User.DomainExperienceYears
                    }
                });

            if (interviewerList == null || !interviewerList.Any())
                throw new AppException(
                    "No interviewers found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            return Ok(interviewerList);
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin,Recruiter,Interviewer")]
        public async Task<IActionResult> GetInterviewerById(int userId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException();

            var loggedInUserId = int.Parse(userIdClaim);

            if (userRoleClaim == "Interviewer" && loggedInUserId != userId)
                throw new AppException(
                    "You don’t have permission to view this interviewer.",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            var interviewer = await _interviewerRepository.GetByFilterAsync(
                i => i.UserId == userId,
                i => new ReviewerInterviewerDetailsDto
                {
                    Id = i.Id,
                    Department = i.Department,

                    User = new UserDto
                    {
                        Id = i.User.Id,
                        FullName = i.User.FullName,
                        Email = i.User.Email,
                        PhoneNumber = i.User.PhoneNumber,
                        Domain = i.User.Domain,
                        DomainExperienceYears = i.User.DomainExperienceYears,
                    },

                    AssignedJobOpenings = i.JobInterviewers
                        .Select(j => new AssignedJobOpeningDto
                        {
                            JobOpeningId = j.JobOpeningId,
                            Title = j.JobOpening.Title,
                            Status = j.JobOpening.Status,
                            Department = j.JobOpening.Department,
                            JobType = j.JobOpening.JobType,
                            CreatedAt = j.JobOpening.CreatedAt,
                            CandidateCount = j.JobOpening.JobCandidates.Count,
                            InterviewerCount = j.JobOpening.JobInterviewers.Count,
                            MinDomainExperience = j.JobOpening.minDomainExperience,
                            Domain = j.JobOpening.Domain
                        })
                        .ToList()
                }
            );

            if (interviewer == null)
                throw new AppException(
                    "Interviewer not found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            return Ok(interviewer);
        }
    }
}
