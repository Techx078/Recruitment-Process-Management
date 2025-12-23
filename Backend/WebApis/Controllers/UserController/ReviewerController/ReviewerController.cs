using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Repository;
using WebApis.Service.ErrorHandlingService;
using WebApis.Service.ErrroHandlingService;

namespace WebApis.Controllers.UserController.ReviewerController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewerController : Controller
    {
        private readonly ICommonRepository<Reviewer> _reviewerRepository;
        public ReviewerController(ICommonRepository<Reviewer> reviewerRepository)
        {
            _reviewerRepository = reviewerRepository;
        }

        [HttpGet("All")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> GetAllReviewer()
        {
            var reviewersList = await _reviewerRepository.GetAllByFilterAsync(
                r => true,
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
                        DomainExperienceYears = r.User.DomainExperienceYears
                    }
                });

            if (reviewersList == null || !reviewersList.Any())
                throw new AppException(
                    "No reviewers found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            return Ok(reviewersList);
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Reviewer,Admin,Recruiter")]
        public async Task<IActionResult> GetReviewerById(int userId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException();

            var loggedInUserId = int.Parse(userIdClaim);

            if (userRoleClaim == "Reviewer" && loggedInUserId != userId)
                throw new AppException(
                    "You don’t have permission to view this reviewer.",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            var reviewerDetails = await _reviewerRepository.GetByFilterAsync(
                r => r.UserId == userId,
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
                        DomainExperienceYears = r.User.DomainExperienceYears
                    },
                    AssignedJobOpenings = r.JobReviewers
                        .Select(j => new AssignedJobOpeningDto
                        {
                            JobOpeningId = j.JobOpeningId,
                            Title = j.JobOpening.Title,
                            Status = j.JobOpening.Status,
                            Department = j.JobOpening.Department,
                            JobType = j.JobOpening.JobType,
                            CreatedAt = j.JobOpening.CreatedAt,
                            CandidateCount = j.JobOpening.JobCandidates.Count,
                            ReviewerCount = j.JobOpening.JobReviewers.Count,
                            MinDomainExperience = j.JobOpening.minDomainExperience,
                            Domain = j.JobOpening.Domain
                        })
                        .ToList()
                });

            if (reviewerDetails == null)
                throw new AppException(
                    "Reviewer not found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            return Ok(reviewerDetails);
        }
    }
}
