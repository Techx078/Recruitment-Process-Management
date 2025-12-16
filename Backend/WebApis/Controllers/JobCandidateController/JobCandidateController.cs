using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos.JobCandidateDtos;
using WebApis.Repository;
using WebApis.Repository.JobCandidateRepository;
using WebApis.Service.ErrroHandlingService;
using WebApis.Service.ValidationService;
using WebApis.Service.ValidationService.JobCandidateValidator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace WebApis.Controllers.JobCandidateController
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobCandidateController : ControllerBase
    {
        private readonly IJobCandidateRepository _jobCandidateRepository;
        private readonly ICommonValidator<JobCandidateCreateDto> _jobCandidateCreateValidator;
        private readonly ICommonRepository<JobOpening> _jobOpeningRepository;
        private readonly ICommonRepository<JobCandidate> _jobCandidateRepositoryCommon;
        private readonly ICommonRepository<Reviewer> _reviewerRepository;
        public JobCandidateController(
            IJobCandidateRepository jobCandidateRepository,
            ICommonValidator<JobCandidateCreateDto> jobCandidateCreateValidator,
            ICommonRepository<JobOpening> jobOpeningRepository,
            ICommonRepository<JobCandidate> jobCandidateRepositoryCommon,
            ICommonRepository<Reviewer> reviewerRepository
        )
        {
            _jobCandidateRepository = jobCandidateRepository;
            _jobCandidateCreateValidator = jobCandidateCreateValidator;
            _jobOpeningRepository = jobOpeningRepository;
            _jobCandidateRepositoryCommon = jobCandidateRepositoryCommon;
            _reviewerRepository = reviewerRepository;
        }
        [HttpPost("create")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CreateJobCandidate([FromBody] JobCandidateCreateDto dto)
        {
            //Validation can be added here
            var result = await _jobCandidateCreateValidator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                return BadRequest(new {
                     errors = result.Errors
                });
            }
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Invalid User.");

            int userId = int.Parse(userIdClaim);

            bool jobOpeningExists = await _jobOpeningRepository.ExistsAsync(
                                     j => j.Id == dto.JobOpeningId && j.CreatedBy.UserId == userId);

            if (!jobOpeningExists)
                throw new UnauthorizedAccessException("You are not authorized to add candidates to this job opening.");

            //call repository method to create jobCandidate
            JobCandidate JobCandidate = await _jobCandidateRepository.CreateJobCandidate(dto);

            return CreatedAtAction(nameof(CreateJobCandidate),
            new { id = JobCandidate.Id },
            new
            {
                Message = "JobCandidate Created SuccessFully"
            });
        }

        [HttpPost("CreateBulk")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CreateJobCandidateBulk([FromBody] JobCandidateCreateBulkDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Request body cannot be null or empty.");
            }
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Invalid User.");
            int userId = int.Parse(userIdClaim);

            bool jobOpeningExists = await _jobOpeningRepository.ExistsAsync(
                                      j => j.Id == dto.JobOpeningId && j.CreatedBy.UserId == userId);

            if (!jobOpeningExists)
                throw new UnauthorizedAccessException("You are not authorized to add candidates to this job opening.");

            int successCount = 0;
            int failureCount = 0;

            List<object> failedItems = new List<object>();
            //validation for each candidate 
            for (int candidateIndex = 0; candidateIndex < dto.CandidateId.Count; candidateIndex++)
            {
                try
                {
                    JobCandidateCreateDto jobCandidateDto = new JobCandidateCreateDto
                    {
                        JobOpeningId = dto.JobOpeningId,
                        CandidateId = dto.CandidateId[candidateIndex],
                        CvPath = dto.CvPath[candidateIndex]
                    };
                    var result = await _jobCandidateCreateValidator.ValidateAsync(jobCandidateDto);
                    if (!result.IsValid)
                    {
                        failureCount++;
                        failedItems.Add(new
                        {
                            CandidateId = jobCandidateDto.CandidateId,
                            Errors = result.Errors
                        });
                        continue; //skip invalid entries
                    }
                    //call repository method to create jobCandidate
                    await _jobCandidateRepository.CreateJobCandidate(jobCandidateDto);
                    successCount++;
                }
                catch (Exception ex)
                {
                    failureCount++;
                    failedItems.Add(new
                    {
                        CandidateId = dto.CandidateId[candidateIndex],
                        Errors = new List<string> { ex.Message }
                    });
                    continue;
                }
            }
                return CreatedAtAction(nameof(CreateJobCandidateBulk),
                new
                {
                    TotalProcessed = dto.CandidateId.Count,
                    SuccessfullyLinked = successCount,
                    Failed = failureCount,
                    FailedItems = failedItems,
                    Message = "JobCandidates Created Successfully"
                });
            }

        [HttpGet("pending-review/{JobOpeningId}")]
        [Authorize(Roles = "Recruiter,Reviewer,Admin")]
        public async Task<IActionResult> GetPendingReviews(int JobOpeningId)
        {
            var jobOpeningExists = await _jobOpeningRepository.ExistsAsync(j => j.Id == JobOpeningId);
            if (!jobOpeningExists)
            {
                throw new KeyNotFoundException("Job Opening not found.");
            }
           
            //check for reviewer is assigned to that jobOpeningId
            var userIdClaim = User.Claims
               .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var userRoleClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            var jobOpening = await _jobOpeningRepository.ExistsAsync(
                j => j.Id == JobOpeningId &&
                 j.JobReviewers.Any(r => r.Reviewer.UserId == loggedInUserId)
            );

            if (userRoleClaim == "Reviewer" && !jobOpening)
                throw new UnauthorizedAccessException("You are unauthorized reviewer for this job opening");

            if( userRoleClaim == "Recruiter")
            {
                var isRecruiterJobOpening = await _jobOpeningRepository.ExistsAsync(
                    j => j.Id == JobOpeningId &&
                    j.CreatedBy.UserId == loggedInUserId
                );
                if (!isRecruiterJobOpening)
                {
                    throw new UnauthorizedAccessException("You are unauthorized recruiter for this job opening");
                }
            }

            var pendingReviewCandidates = await _jobCandidateRepositoryCommon
                .GetAllByFilterAsync(
                    c =>
                    c.JobOpeningId == JobOpeningId &&
                    c.Status == "Applied",
                    c => new PendingReviewCandidateDto
                    {
                        JobCandidateId = c.Id,
                        JobOpeningId = c.JobOpening.Id,
                        CandidateName = c.Candidate.User.FullName,
                        CandidateEmail = c.Candidate.User.Email,
                        JobTitle = c.JobOpening.Title,
                        CvPath = c.CvPath,
                        AppliedAt = c.CreatedAt,
                        UserId = c.Candidate.UserId
                    }

                );
            if( pendingReviewCandidates == null)
            {
                throw new KeyNotFoundException("no candidate exits or wrong jobOpeningId");
            }
            return Ok(pendingReviewCandidates);
        }

        [HttpPut("review-status/{jobCandidateId}")]
        [Authorize(Roles = "Reviewer")]
        public async Task<IActionResult> UpdateReviewStatus(
            int jobCandidateId,
            [FromBody] ReviewDecisionDto dto)
        {
            //get jobCandidate by id
            var jobCandidate = await _jobCandidateRepositoryCommon
                .GetByFilterAsync(c => c.Id == jobCandidateId);
            //check if jobCandidate exists
            if (jobCandidate == null)
                throw new KeyNotFoundException("Job candidate not found");

            //check if logged in user is authorized reviewer for this jobCandidate
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var userRoleClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                return Unauthorized(new { message = "Invalid token" });

            int loggedInUserId = int.Parse(userIdClaim);
           
            if (userRoleClaim == "Reviewer")
            {
                var isReviewerAssigned = await _jobOpeningRepository.ExistsAsync(
                    j => j.Id == jobCandidate.JobOpeningId &&
                     j.JobReviewers.Any(r => r.Reviewer.UserId == loggedInUserId)
                );
                if (!isReviewerAssigned)
                    throw new UnauthorizedAccessException("You are unauthorized reviewer for this job candidate");

                //set reviewerId
                var loggedInReviewerId = await _reviewerRepository
                    .GetByFilterAsync(
                        r => r.UserId == loggedInUserId,
                        r => r.Id
                    );
                if( loggedInReviewerId == null)
                    throw new UnauthorizedAccessException("Reviewer profile not found for the logged-in user.");
                jobCandidate.ReviewerId = loggedInReviewerId;
            }

            // Only candidates in Applied state can be reviewed
            if (jobCandidate.Status != "Applied")
                throw new AppException("Only candidates in 'Applied' state can be reviewed.", 400);

            //add comments
            jobCandidate.ReviewerComment = dto.ReviewerComment;
            jobCandidate.UpdatedAt = DateTime.UtcNow;

            //if approved move to technical round else rejected
            if (dto.IsApproved)
            {
                jobCandidate.Status = "Reviewed";
                jobCandidate.IsNextTechnicalRound = true;
                jobCandidate.IsNextHrRound = false;
                jobCandidate.RoundNumber = 0;
            }
            else
            {
                jobCandidate.Status = "Rejected";
                jobCandidate.IsNextTechnicalRound = false;
                jobCandidate.IsNextHrRound = false;
            }

            await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);
           
            return Ok(new
            {
                message = dto.IsApproved
                    ? "Candidate approved and sent to technical pool"
                    : "Candidate rejected successfully"
            });
        }
    }
}

