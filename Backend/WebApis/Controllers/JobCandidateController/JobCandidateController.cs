using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Dtos.JobCandidateDtos;
using WebApis.Repository;
using WebApis.Repository.JobCandidateRepository;
using WebApis.Service.ErrroHandlingService;
using WebApis.Service.ValidationService;

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
        private readonly ICommonRepository<Interviewer> _interviewerRepository;
        private readonly ICommonRepository<JobInterview> _jobInterviewRepository;
        public JobCandidateController(
            IJobCandidateRepository jobCandidateRepository,
            ICommonValidator<JobCandidateCreateDto> jobCandidateCreateValidator,
            ICommonRepository<JobOpening> jobOpeningRepository,
            ICommonRepository<JobCandidate> jobCandidateRepositoryCommon,
            ICommonRepository<Reviewer> reviewerRepository,
            ICommonRepository<Interviewer> interviewerRepository,
            ICommonRepository<JobInterview> jobInterviewRepository
        )
        {
            _jobCandidateRepository = jobCandidateRepository;
            _jobCandidateCreateValidator = jobCandidateCreateValidator;
            _jobOpeningRepository = jobOpeningRepository;
            _jobCandidateRepositoryCommon = jobCandidateRepositoryCommon;
            _reviewerRepository = reviewerRepository;
            _interviewerRepository = interviewerRepository;
            _jobInterviewRepository = jobInterviewRepository;
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

        [HttpGet("get/{jobCandidateId}")]
        [Authorize(Roles = "Recruiter,Reviewer,Admin,Interviewer")]
        public async Task<IActionResult> GetJobCandidateById( int jobCandidateId )
        {
            var jobCandidate = await _jobCandidateRepositoryCommon.GetByFilterAsync(j => j.Id == jobCandidateId);
            if (jobCandidate == null)
                throw new KeyNotFoundException("job Candidate Not found");
            return Ok(jobCandidate);
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

            if (!int.TryParse(userIdClaim, out int loggedInUserId))
                return Unauthorized("Invalid user id");

            var isReviewerAssigned = await _jobOpeningRepository.ExistsAsync(
                j => j.Id == JobOpeningId &&
                 j.JobReviewers.Any(r => r.Reviewer.UserId == loggedInUserId)
            );

            if (userRoleClaim == "Reviewer" && !isReviewerAssigned)
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

        [HttpGet("pool/technical/{jobOpeningId}")]
        [Authorize(Roles = "Recruiter,Admin,Interviewer")]
        public async Task<IActionResult> GetTechnicalInterviewPool(int jobOpeningId)
        {
            var jobOpeningExists = await _jobOpeningRepository.ExistsAsync(j => j.Id == jobOpeningId);
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

            if (!int.TryParse(userIdClaim, out int loggedInUserId))
                throw new UnauthorizedAccessException("user id not valid");

            var isInterviewerAssigned = await _jobOpeningRepository.ExistsAsync(
                j => j.Id == jobOpeningId &&
                 j.JobInterviewers.Any(i => i.Interviewer.UserId == loggedInUserId)
            );

            if (userRoleClaim == "Interviewer" && !isInterviewerAssigned)
                throw new UnauthorizedAccessException("You are unauthorized reviewer for this job opening");

            if (userRoleClaim == "Recruiter")
            {
                var isRecruiterJobOpening = await _jobOpeningRepository.ExistsAsync(
                    j => j.Id == jobOpeningId &&
                    j.CreatedBy.UserId == loggedInUserId
                );
                if (!isRecruiterJobOpening)
                {
                    throw new UnauthorizedAccessException("You are unauthorized recruiter for this job opening");
                }
            }
            var candidates = await _jobCandidateRepositoryCommon.GetByOrderWithSelectorAsync
                (c =>
                    c.JobOpeningId == jobOpeningId &&
                    c.IsNextTechnicalRound &&
                    (c.Status == "Reviewed" || c.Status == "WaitForInterView")
                ,
                c => new TechnicalPoolCandidateDto
                {
                    jobCandidateId = c.Id,
                    jobOpeningId = c.JobOpeningId,
                    candidateName = c.Candidate.User.FullName,
                    jobTitle = c.JobOpening.Title,
                    roundNumber = c.RoundNumber,
                    lastUpdatedAt = c.UpdatedAt,
                    candidateId = c.CandidateId,
                    userId = c.Candidate.UserId,
                },
                q => q
                      .OrderBy(c => c.RoundNumber)
                      .ThenBy(c => c.UpdatedAt)
                );

            return Ok(candidates);
        }

        [HttpGet("my-scheduled/{jobOpeningId}")]
        [Authorize(Roles = "Recruiter,Admin,Interviewer")]
        public async Task<IActionResult> GetMyScheduledInterviews(int jobOpeningId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            var interviewer = await _interviewerRepository
                .GetByFilterAsync(
                    i => i.UserId == loggedInUserId,
                    i => new { i.Id }
                );

            if (interviewer == null)
                return Unauthorized("Interviewer profile not found");

            var isAssigned = await _jobOpeningRepository.ExistsAsync(
                j => j.Id == jobOpeningId &&
                     j.JobInterviewers.Any(i => i.InterviewerId == interviewer.Id)
            );

            if (!isAssigned)
                return Forbid("You are not assigned to this job opening");

            var interviews = await _jobInterviewRepository
                .GetAllByFilterAsync(
                    i => i.InterviewerId == interviewer.Id
                      && !i.IsCompleted
                      && i.JobCandidate.JobOpeningId == jobOpeningId,
                    i => new MyScheduledInterviewDto
                    {
                        JobInterviewId = i.Id,
                        JobCandidateId = i.JobCandidateId,
                        CandidateName = i.JobCandidate.Candidate.User.FullName,
                        CandidateEmail = i.JobCandidate.Candidate.User.Email,
                        InterviewType = i.InterviewType,
                        RoundNumber = i.RoundNumber,
                        ScheduledAt = i.ScheduledAt,
                        MeetingLink = i.MeetingLink
                    }
                );

            return Ok(interviews);
        }


        [HttpPost("schedule/{jobCandidateId}")]
        [Authorize(Roles = "Interviewer")]
        public async Task<IActionResult> ScheduleInterview(
            int jobCandidateId,
            [FromBody] ScheduleInterviewDto dto)
        {
            //get jobCandidate by id
            var jobCandidate = await _jobCandidateRepositoryCommon
                .GetByFilterAsync(c => c.Id == jobCandidateId)
                ?? throw new KeyNotFoundException("Job candidate not found");
            

            //check if logged in user is authorized reviewer for this jobCandidate
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Invalid token" });

            if (!int.TryParse(userIdClaim, out int loggedInUserId))
                throw new UnauthorizedAccessException("Invalid user id in token");

            var isInterviewerAssigned = await _jobOpeningRepository.ExistsAsync(
                j => j.Id == jobCandidate.JobOpeningId &&
                j.JobInterviewers.Any(r => r.Interviewer.UserId == loggedInUserId)
            );
            if (!isInterviewerAssigned)
                throw new UnauthorizedAccessException("You are unauthorized reviewer for this job candidate");

            if (dto.InterviewDate <= DateTime.UtcNow)
                return BadRequest(new
                {
                    errros = new[] { "Interview date must be in the future" }
                });

            if( jobCandidate.Status == "ScheduledInterview")
            {
                return BadRequest(new
                {
                    erros = new[] {"Cnadidate already scheduled for this round"}
                });
            }
            var hasActiveInterview = await _jobInterviewRepository.ExistsAsync(
                                        i => i.JobCandidateId == jobCandidate.Id && i.IsCompleted == false
                                    );
            if (hasActiveInterview)
                return BadRequest(new
                {
                    error = new[] { "Candidate already has an active interview" }
                });

            if (!jobCandidate.IsNextTechnicalRound && !jobCandidate.IsNextHrRound)
                return BadRequest(new
                {
                    errors = new[] { "Candidate is not ready for interview" }
                });
            //set interviewerId
            var InterviewerId = await _interviewerRepository.GetByFilterAsync(
                                            r => r.UserId == loggedInUserId,
                                            r => r.Id
                                        );
            if (InterviewerId == null)
                throw new UnauthorizedAccessException("Interviewer profile not found for the logged-in user.");

            var interview = new JobInterview
            {
                JobCandidateId = jobCandidate.Id,
                InterviewerId = InterviewerId,
                RoundNumber = jobCandidate.RoundNumber + 1,
                ScheduledAt = dto.InterviewDate,
                InterviewType = jobCandidate.IsNextHrRound ? "HR" : "Technical",
                MeetingLink = dto.MeetingLink,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsCompleted = false
            };

            await _jobInterviewRepository.AddAsync(interview);

            jobCandidate.Status = "ScheduledInterview";
            jobCandidate.UpdatedAt = DateTime.UtcNow;

            await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

            return Ok(new
            {
                message = "Interview scheduled successfully"
            });
        }

        [HttpPost("feedback/{jobCandidateId}")]
        [Authorize(Roles = "Interviewer")]
        public async Task<IActionResult> SubmitInterviewFeedback(
            int jobCandidateId,
            [FromBody] InterviewFeedbackDto dto)
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            var interviewer = await _interviewerRepository.GetByFilterAsync(i => i.UserId == loggedInUserId);
           
            if (interviewer == null)
                throw new UnauthorizedAccessException("Interviewer profile not found");

            var jobCandidate = await _jobCandidateRepositoryCommon.GetWithIncludeAsync(
                    c => c.Id == jobCandidateId,
                    c => c,
                    "JobInterviews"
                );

            if (jobCandidate == null)
                return NotFound("Job candidate not found");

            if (jobCandidate.Status != "ScheduledInterview")
                return BadRequest("Candidate is not in scheduled interview state");

            var interview = jobCandidate.JobInterviews
                .FirstOrDefault(i =>
                    i.IsCompleted == false &&
                    i.InterviewerId == interviewer.Id);

            if (interview == null)
                return Forbid("No active interview found for this interviewer");

            if (interview.InterviewType == "HR" && dto.NextStep == "Technical")
                return BadRequest("HR cannot move candidate back to technical");

            interview.Feedback = dto.Feedback;
            interview.Marks = dto.Marks;
            interview.IsPassed = dto.IsPassed;
            interview.IsCompleted = true;
            interview.UpdatedAt = DateTime.UtcNow;

            jobCandidate.UpdatedAt = DateTime.UtcNow;

            if (!dto.IsPassed)
            {
                jobCandidate.Status = "Rejected";
                jobCandidate.IsNextTechnicalRound = false;
                jobCandidate.IsNextHrRound = false;
            }
            else
            {
                jobCandidate.RoundNumber++;

                switch (dto.NextStep)
                {
                    case "Technical":
                        jobCandidate.Status = "WaitForInterView";
                        jobCandidate.IsNextTechnicalRound = true;
                        jobCandidate.IsNextHrRound = false;
                        break;

                    case "HR":
                        jobCandidate.Status = "WaitForInterView";
                        jobCandidate.IsNextTechnicalRound = false;
                        jobCandidate.IsNextHrRound = true;
                        break;

                    case "Finish":
                        jobCandidate.Status = "Shortlisted";
                        jobCandidate.IsNextTechnicalRound = false;
                        jobCandidate.IsNextHrRound = false;
                        break;

                    default:
                        return BadRequest("Invalid next step");
                }
            }

            jobCandidate.UpdatedAt = DateTime.UtcNow;
            await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

            return Ok(new { message = "Interview feedback submitted successfully" });
        }
    }
}