using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Dtos.JobCandidateDtos;
using WebApis.Repository;
using WebApis.Repository.JobCandidateRepository;
using WebApis.Service.ErrorHandlingService;
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
            var validationResult = await _jobCandidateCreateValidator.ValidateAsync(dto);
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
                throw new UnauthorizedAccessException("invalid token !");

            int userId = int.Parse(userIdClaim);

            bool jobOpeningExists = await _jobOpeningRepository.ExistsAsync(
                                      j => j.Id == dto.JobOpeningId && j.CreatedBy.UserId == userId);
            if (!jobOpeningExists)
                throw new AppException(
                    "You are not authorized to add candidates to this job opening.",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            var jobCandidate = await _jobCandidateRepository.CreateJobCandidate(dto);

            return CreatedAtAction(nameof(CreateJobCandidate),
                new { id = jobCandidate.Id },
                new
                {
                    Message = "JobCandidate created successfully"
                });
        }

        [HttpPost("createBulk")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CreateJobCandidateBulk([FromBody] JobCandidateCreateBulkDto dto)
        {
            if (dto == null)
                throw new AppException(
                    "Request body cannot be null or empty.",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException();

            int userId = int.Parse(userIdClaim);

            bool jobOpeningExists = await _jobOpeningRepository.ExistsAsync(
                                       j => j.Id == dto.JobOpeningId && j.CreatedBy.UserId == userId);
            if (!jobOpeningExists)
                throw new AppException(
                    "You are not authorized to add candidates to this job opening.",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            int successCount = 0;
            int failureCount = 0;
            var failedItems = new List<object>();

            for (int i = 0; i < dto.CandidateId.Count; i++)
            {
                try
                {
                    var jobCandidateDto = new JobCandidateCreateDto
                    {
                        JobOpeningId = dto.JobOpeningId,
                        CandidateId = dto.CandidateId[i],
                        CvPath = dto.CvPath[i]
                    };

                    var validationResult = await _jobCandidateCreateValidator.ValidateAsync(jobCandidateDto);
                    if (!validationResult.IsValid)
                    {
                        failureCount++;
                        failedItems.Add(new
                        {
                            CandidateId = jobCandidateDto.CandidateId,
                            Errors = validationResult.Errors
                        });
                        continue;
                    }

                    await _jobCandidateRepository.CreateJobCandidate(jobCandidateDto);
                    successCount++;
                }
                catch (Exception ex)
                {
                    failureCount++;
                    failedItems.Add(new
                    {
                        CandidateId = dto.CandidateId[i],
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
                    Message = "JobCandidates processed successfully"
                });
        }

        [HttpGet("get/{jobCandidateId}")]
        [Authorize(Roles = "Recruiter,Reviewer,Admin,Interviewer")]
        public async Task<IActionResult> GetJobCandidateById(int jobCandidateId)
        {
            var jobCandidate = await _jobCandidateRepositoryCommon.GetByFilterAsync(j => j.Id == jobCandidateId);
            if (jobCandidate == null)
                throw new AppException(
                    "Job candidate not found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            return Ok(jobCandidate);
        }

        [HttpGet("pool/review/{jobOpeningId}")]
        [Authorize(Roles = "Recruiter,Reviewer,Admin")]
        public async Task<IActionResult> GetPendingReviews(int jobOpeningId)
        {
            var jobOpeningExists = await _jobOpeningRepository.ExistsAsync(j => j.Id == jobOpeningId);
            if (!jobOpeningExists)
                throw new AppException(
                    "Job opening not found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            if (userRoleClaim == "Reviewer")
            {
                var isReviewerAssigned = await _jobOpeningRepository.ExistsAsync(
                    j => j.Id == jobOpeningId &&
                         j.JobReviewers.Any(r => r.Reviewer.UserId == loggedInUserId)
                );

                if (!isReviewerAssigned)
                    throw new AppException(
                        "You are not authorized to review this job opening.",
                        ErrorCodes.Forbidden,
                        StatusCodes.Status403Forbidden
                    );
            }

            if (userRoleClaim == "Recruiter")
            {
                var isRecruiterJobOpening = await _jobOpeningRepository.ExistsAsync(
                    j => j.Id == jobOpeningId &&
                         j.CreatedBy.UserId == loggedInUserId
                );

                if (!isRecruiterJobOpening)
                    throw new AppException(
                        "You are not authorized to manage this job opening.",
                        ErrorCodes.Forbidden,
                        StatusCodes.Status403Forbidden
                    );
            }

            var pendingReviewCandidates = await _jobCandidateRepositoryCommon
                .GetAllByFilterAsync(
                    c => c.JobOpeningId == jobOpeningId && c.Status == "Applied",
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

            if (pendingReviewCandidates == null || !pendingReviewCandidates.Any())
                throw new AppException(
                    "No candidates found for this job opening.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            return Ok(pendingReviewCandidates);
        }

        [HttpPut("review/{jobCandidateId}")]
        [Authorize(Roles = "Reviewer")]
        public async Task<IActionResult> UpdateReviewStatus(int jobCandidateId, [FromBody] ReviewDecisionDto dto)
        {
            var jobCandidate = await _jobCandidateRepositoryCommon.GetByFilterAsync(c => c.Id == jobCandidateId);
            if (jobCandidate == null)
                throw new AppException(
                    "Job candidate not found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException();

            int loggedInUserId = int.Parse(userIdClaim);

            var isReviewerAssigned = await _jobOpeningRepository.ExistsAsync(
                j => j.Id == jobCandidate.JobOpeningId &&
                     j.JobReviewers.Any(r => r.Reviewer.UserId == loggedInUserId)
            );

            if (!isReviewerAssigned)
                throw new AppException(
                    "You are not authorized to review this candidate.",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            var loggedInReviewerId = await _reviewerRepository.GetByFilterAsync(
                r => r.UserId == loggedInUserId,
                r => r.Id
            );

            if (loggedInReviewerId == null)
                throw new AppException(
                    "Reviewer profile not found for the logged-in user.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            jobCandidate.ReviewerId = loggedInReviewerId;

            if (jobCandidate.Status != "Applied")
                throw new AppException(
                    "Only candidates in 'Applied' state can be reviewed.",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            jobCandidate.ReviewerComment = dto.ReviewerComment;
            jobCandidate.UpdatedAt = DateTime.UtcNow;

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
                    ? "Candidate approved and sent to technical pool."
                    : "Candidate rejected successfully."
            });
        }

        [HttpGet("pool/technical/{jobOpeningId}")]
        [Authorize(Roles = "Recruiter,Admin,Interviewer")]
        public async Task<IActionResult> GetTechnicalInterviewPool(int jobOpeningId)
        {
            var jobOpeningExists = await _jobOpeningRepository.ExistsAsync(j => j.Id == jobOpeningId);
            if (!jobOpeningExists)
                throw new AppException(
                    "Job opening not found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException();

            int loggedInUserId = int.Parse(userIdClaim);

            if (userRoleClaim == "Interviewer")
            {
                var isInterviewerAssigned = await _jobOpeningRepository.ExistsAsync(
                    j => j.Id == jobOpeningId &&
                         j.JobInterviewers.Any(i => i.Interviewer.UserId == loggedInUserId)
                );

                if (!isInterviewerAssigned)
                    throw new AppException(
                        "You are not authorized to access this technical pool.",
                        ErrorCodes.Forbidden,
                        StatusCodes.Status403Forbidden
                    );
            }

            if (userRoleClaim == "Recruiter")
            {
                var isRecruiterJobOpening = await _jobOpeningRepository.ExistsAsync(
                    j => j.Id == jobOpeningId &&
                         j.CreatedBy.UserId == loggedInUserId
                );

                if (!isRecruiterJobOpening)
                    throw new AppException(
                        "You are not authorized to access this technical pool.",
                        ErrorCodes.Forbidden,
                        StatusCodes.Status403Forbidden
                    );
            }

            var candidates = await _jobCandidateRepositoryCommon.GetByOrderWithSelectorAsync(
                c => c.JobOpeningId == jobOpeningId &&
                     c.IsNextTechnicalRound &&
                     (c.Status == "Reviewed" || c.Status == "WaitForInterView"),
                c => new TechnicalPoolCandidateDto
                {
                    jobCandidateId = c.Id,
                    jobOpeningId = c.JobOpeningId,
                    candidateName = c.Candidate.User.FullName,
                    jobTitle = c.JobOpening.Title,
                    roundNumber = c.RoundNumber,
                    lastUpdatedAt = c.UpdatedAt,
                    candidateId = c.CandidateId,
                    userId = c.Candidate.UserId
                },
                q => q.OrderBy(c => c.RoundNumber).ThenBy(c => c.UpdatedAt)
            );

            return Ok(candidates);
        }

        [HttpGet("pool/my-scheduled/{jobOpeningId}")]
        [Authorize(Roles = "Recruiter,Admin,Interviewer")]
        public async Task<IActionResult> GetMyScheduledInterviews(int jobOpeningId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            var interviewer = await _interviewerRepository.GetByFilterAsync(
                i => i.UserId == loggedInUserId,
                i => i.Id
            );

            if (interviewer == null)
                throw new AppException("Interviewer profile not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var isAssigned = await _jobOpeningRepository.ExistsAsync(
                j => j.Id == jobOpeningId &&
                     j.JobInterviewers.Any(i => i.InterviewerId == interviewer)
            );

            if (!isAssigned)
                throw new AppException("You are not assigned to this job opening", ErrorCodes.Forbidden, StatusCodes.Status403Forbidden);

            var interviews = await _jobInterviewRepository.GetAllByFilterAsync(
                i => i.InterviewerId == interviewer &&
                     !i.IsCompleted &&
                     i.JobCandidate.JobOpeningId == jobOpeningId,
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

        [HttpPut("schedule/{jobCandidateId}")]
        [Authorize(Roles = "Interviewer")]
        public async Task<IActionResult> ScheduleInterview(int jobCandidateId, [FromBody] ScheduleInterviewDto dto)
        {
            var jobCandidate = await _jobCandidateRepositoryCommon.GetByFilterAsync(c => c.Id == jobCandidateId)
                               ?? throw new AppException("Job candidate not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            var isInterviewerAssigned = await _jobOpeningRepository.ExistsAsync(
                j => j.Id == jobCandidate.JobOpeningId &&
                     j.JobInterviewers.Any(r => r.Interviewer.UserId == loggedInUserId)
            );
            if (!isInterviewerAssigned)
                throw new AppException("You are not authorized for this job candidate", ErrorCodes.Forbidden, StatusCodes.Status403Forbidden);

            if (dto.InterviewDate <= DateTime.UtcNow)
                throw new AppException("Interview date must be in the future", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            if (jobCandidate.Status == "ScheduledInterview")
                throw new AppException("Candidate already scheduled for this round", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            var hasActiveInterview = await _jobInterviewRepository.ExistsAsync(
                i => i.JobCandidateId == jobCandidate.Id && !i.IsCompleted
            );
            if (hasActiveInterview)
                throw new AppException("Candidate already has an active interview", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            if (!jobCandidate.IsNextTechnicalRound && !jobCandidate.IsNextHrRound)
                throw new AppException("Candidate is not ready for interview", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            var interviewerId = await _interviewerRepository.GetByFilterAsync(
                r => r.UserId == loggedInUserId,
                r => r.Id
            );
            if (interviewerId == 0)
                throw new AppException("Interviewer profile not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var interview = new JobInterview
            {
                JobCandidateId = jobCandidate.Id,
                InterviewerId = interviewerId,
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

            return Ok(new { message = "Interview scheduled successfully" });
        }

        [HttpPut("Interview-feedback/{jobCandidateId}")]
        [Authorize(Roles = "Interviewer")]
        public async Task<IActionResult> SubmitInterviewFeedback(int jobCandidateId, [FromBody] InterviewFeedbackDto dto)
        {
            if (dto.IsPassed && string.IsNullOrEmpty(dto.NextStep))
                throw new AppException("NextStep is required when interview is passed", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            var interviewer = await _interviewerRepository.GetByFilterAsync(i => i.UserId == loggedInUserId)
                              ?? throw new AppException("Interviewer profile not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var jobCandidate = await _jobCandidateRepositoryCommon.GetWithIncludeAsync(
                c => c.Id == jobCandidateId,
                c => c,
                "JobInterviews"
            ) ?? throw new AppException("Job candidate not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var isInterviewerAssigned = await _jobOpeningRepository.ExistsAsync(
                j => j.Id == jobCandidate.JobOpeningId &&
                     j.JobInterviewers.Any(i => i.InterviewerId == interviewer.Id)
            );

            if (!isInterviewerAssigned)
                throw new AppException("You are not assigned to this job opening", ErrorCodes.Forbidden, StatusCodes.Status403Forbidden);

            if (jobCandidate.Status != "ScheduledInterview")
                throw new AppException("Candidate is not in scheduled interview state", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            var interview = jobCandidate?.JobInterviews?.FirstOrDefault(i => !i.IsCompleted && i.InterviewerId == interviewer.Id)
                            ?? throw new AppException("No active interview found for this interviewer", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            if (interview.InterviewType == "HR" && dto.NextStep == "Technical")
                throw new AppException("HR cannot move candidate back to technical", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

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
                        throw new AppException("Invalid next step", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);
                }
            }

            await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

            return Ok(new { message = "Interview feedback submitted successfully" });
        }

        [HttpGet("pool/hr/{jobOpeningId}")]
        [Authorize(Roles = "Interviewer,Recruiter,Admin")]
        public async Task<IActionResult> GetHrPool(int jobOpeningId)
        {
            if (!await _jobOpeningRepository.ExistsAsync(j => j.Id == jobOpeningId))
                throw new AppException("Job opening not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException("Invalid token");
            int loggedInUserId = int.Parse(userIdClaim);
            if (userRoleClaim == "Interviewer")
            {
                var hrId = await _interviewerRepository.GetByFilterAsync(
                    i => i.UserId == loggedInUserId && i.Department == "HR",
                    i => i.Id
                );

                if (hrId == 0)
                    throw new AppException("HR profile not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

                var isHrAssigned = await _jobOpeningRepository.ExistsAsync(
                    j => j.Id == jobOpeningId &&
                         j.JobInterviewers.Any(i => i.InterviewerId == hrId && i.Interviewer.Department == "HR")
                );

                if (!isHrAssigned)
                    throw new AppException("You are not assigned to this job opening", ErrorCodes.Forbidden, StatusCodes.Status403Forbidden);
            }
            if (userRoleClaim == "Recruiter")
            {
                var isRecruiterJobOpening = await _jobOpeningRepository.ExistsAsync(
                    j => j.Id == jobOpeningId &&
                         j.CreatedBy.UserId == loggedInUserId
                );

                if (!isRecruiterJobOpening)
                    throw new AppException(
                        "You are not authorized to access this technical pool.",
                        ErrorCodes.Forbidden,
                        StatusCodes.Status403Forbidden
                    );
            }
            var candidates = await _jobCandidateRepositoryCommon.GetAllByFilterAsync(
                c => c.JobOpeningId == jobOpeningId &&
                     c.Status == "WaitForInterView" &&
                     c.IsNextHrRound,
                c => new HrPoolCandidateDto
                {
                    JobCandidateId = c.Id,
                    CandidateId = c.CandidateId,
                    UserId = c.Candidate.UserId,
                    CandidateName = c.Candidate.User.FullName,
                    Email = c.Candidate.User.Email,
                    RoundCompleted = c.RoundNumber,
                    JobTitle = c.JobOpening.Title,
                    AppliedAt = c.CreatedAt
                }
            );

            return Ok(candidates);
        }

        [HttpGet("history/{jobCandidateId}")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> GetInterviewHistory(int jobCandidateId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException("Invalid token");

            var userId = int.Parse(userIdClaim);

            var jobCandidate = await _jobCandidateRepositoryCommon.GetWithIncludeAsync(
                c => c.Id == jobCandidateId,
                c => c,
                "Candidate.User",
                "JobOpening.CreatedBy",
                "JobOpening.JobReviewers.Reviewer",
                "JobOpening.JobInterviewers.Interviewer",
                "JobInterviews.Interviewer.User"
            ) ?? throw new AppException("Job candidate not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            bool hasAccess = userRoleClaim switch
            {
                "Recruiter" => jobCandidate.JobOpening.CreatedBy.UserId == userId,
                "Admin" => true,
                _ => false
            };

            if (!hasAccess)
                throw new AppException("You are not authorized to view this candidate", ErrorCodes.Forbidden, StatusCodes.Status403Forbidden);

            var timeline = new InterviewTimelineDto
            {
                JobCandidateId = jobCandidate.Id,
                CandidateName = jobCandidate.Candidate.User.FullName,
                JobTitle = jobCandidate.JobOpening.Title,
                CurrentStatus = jobCandidate.Status
            };

            timeline.Timeline.Add(new TimelineEventDto
            {
                Title = "Application Submitted",
                Description = "Candidate applied for the job",
                Date = jobCandidate.CreatedAt,
                EventType = "Applied"
            });

            if (jobCandidate.ReviewerId != null)
            {
                timeline.Timeline.Add(new TimelineEventDto
                {
                    Title = "Profile Reviewed",
                    Description = jobCandidate.ReviewerComment,
                    Date = jobCandidate.UpdatedAt,
                    EventType = "Review"
                });
            }

            if (jobCandidate.JobInterviews != null)
            {
                foreach (var interview in jobCandidate.JobInterviews.OrderBy(i => i.RoundNumber))
                {
                    timeline.Timeline.Add(new TimelineEventDto
                    {
                        Title = $"{interview.InterviewType} Interview (Round {interview.RoundNumber})",
                        Description = interview.IsCompleted
                            ? $"Result: {(interview.IsPassed == true ? "Passed" : "Rejected")}"
                            : "Scheduled",
                        Date = interview.ScheduledAt,
                        EventType = "Interview"
                    });
                }
            }

            if (jobCandidate.Status == "Shortlisted")
            {
                timeline.Timeline.Add(new TimelineEventDto
                {
                    Title = "Candidate Shortlisted",
                    Description = "Candidate cleared all rounds",
                    Date = jobCandidate.UpdatedAt,
                    EventType = "Final"
                });
            }
            else if (jobCandidate.Status == "Rejected")
            {
                timeline.Timeline.Add(new TimelineEventDto
                {
                    Title = "Candidate Rejected",
                    Description = "Candidate did not clear the interview process",
                    Date = jobCandidate.UpdatedAt,
                    EventType = "Final"
                });
            }

            timeline.Timeline = timeline.Timeline.OrderBy(t => t.Date).ToList();

            return Ok(timeline);
        }

        [HttpGet("pool/shortlist/{jobOpeningId}")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> GetFinalPool(int jobOpeningId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException("Invalid token");

            var userId = int.Parse(userIdClaim);

            var jobOpening = await _jobOpeningRepository.GetWithIncludeAsync(
                j => j.Id == jobOpeningId,
                j => j,
                "CreatedBy",
                "JobCandidates.Candidate.User",
                "JobCandidates.JobInterviews"
            ) ?? throw new AppException("Job opening not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            if (userRoleClaim == "Recruiter" && jobOpening.CreatedBy.UserId != userId)
                throw new AppException("You are not authorized to view this job opening", ErrorCodes.Forbidden, StatusCodes.Status403Forbidden);

            var candidates = jobOpening.JobCandidates
                .Where(c => c.Status == "Shortlisted" || c.Status == "Selected")
                .Select(c => new FinalPoolCandidateDto
                {
                    JobCandidateId = c.Id,
                    CandidateName = c.Candidate.User.FullName,
                    Email = c.Candidate.User.Email,
                    TotalRounds = c.RoundNumber,
                    LastInterviewType = c.JobInterviews
                        .OrderByDescending(i => i.RoundNumber)
                        .Select(i => i.InterviewType)
                        .FirstOrDefault(),
                    LastInterviewDate = c.JobInterviews
                        .OrderByDescending(i => i.RoundNumber)
                        .Select(i => i.ScheduledAt)
                        .FirstOrDefault(),
                    Status = c.Status
                })
                .ToList();

            return Ok(candidates);
        }

        [HttpPut("select/{jobCandidateId}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> SelectCandidate(int jobCandidateId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new UnauthorizedAccessException("Invalid token");

            int userId = int.Parse(userIdClaim);

            var jobCandidate = await _jobCandidateRepositoryCommon.GetWithIncludeAsync(
                c => c.Id == jobCandidateId,
                c => c,
                "JobOpening.CreatedBy"
            ) ?? throw new AppException("Job candidate not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            if (jobCandidate.Status != "Shortlisted")
                throw new AppException("Only shortlisted candidates can be selected", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            if (jobCandidate.JobOpening.CreatedBy.UserId != userId)
                throw new AppException("You are not authorized to select this candidate", ErrorCodes.Forbidden, StatusCodes.Status403Forbidden);

            jobCandidate.Status = "Selected";
            jobCandidate.UpdatedAt = DateTime.UtcNow;

            await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

            return Ok(new
            {
                message = "Candidate selected successfully"
            });
        }

        [HttpPut("send-offer/{jobCandidateId}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> SendOffer(
            int jobCandidateId,
            [FromBody] SendOfferDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException();

            int userId = int.Parse(userIdClaim);

            var jobCandidate = await _jobCandidateRepositoryCommon
                .GetWithIncludeAsync(
                    c => c.Id == jobCandidateId,
                    c => c,
                    "JobOpening.CreatedBy"
                );

            if (jobCandidate == null)
                throw new AppException(
                    "Job candidate not found",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            if (jobCandidate.Status != "Selected")
                throw new AppException(
                    "Offer can be sent only to selected candidates",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            if (jobCandidate.JobOpening.CreatedBy.UserId != userId)
                throw new AppException(
                    "You are not authorized to send offer for this candidate",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            if (dto.OfferExpiryDate <= DateTime.UtcNow)
                throw new AppException(
                    "Offer expiry must be in the future",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            jobCandidate.OfferExpiryDate = dto.OfferExpiryDate;
            jobCandidate.Status = "OfferSent";
            jobCandidate.UpdatedAt = DateTime.UtcNow;

            await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

            return Ok(new
            {
                message = "Offer sent successfully"
            });
        }


        [HttpPut("respond-offer/{jobCandidateId}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> OfferResponse(
             int jobCandidateId,
             [FromBody] OfferResponseDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException();

            int userId = int.Parse(userIdClaim);

            var jobCandidate = await _jobCandidateRepositoryCommon
                .GetWithIncludeAsync(
                    c => c.Id == jobCandidateId,
                    c => c,
                    "Candidate.User"
                );

            if (jobCandidate == null)
                throw new AppException(
                    "Job candidate not found",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            if (jobCandidate.Candidate.UserId != userId)
                throw new AppException(
                    "You are not authorized to respond to this offer",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            if (jobCandidate.Status != "OfferSent")
                throw new AppException(
                    "No active offer found",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            if (jobCandidate.OfferExpiryDate < DateTime.UtcNow)
                throw new AppException(
                    "Offer has expired",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            if (dto.IsAccepted)
            {
                jobCandidate.Status = "OfferAccepted";
            }
            else
            {
                jobCandidate.Status = "OfferRejectedByCandidate";
                jobCandidate.OfferRejectionReason = dto.RejectionReason;
            }

            jobCandidate.UpdatedAt = DateTime.UtcNow;

            await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

            return Ok(new
            {
                message = dto.IsAccepted
                    ? "Offer accepted successfully"
                    : "Offer rejected successfully"
            });
        }

        [HttpGet("pool/offerSend/{jobOpeningId}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetOfferedCandidates(int jobOpeningId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var jobOpening = await _jobOpeningRepository.GetWithIncludeAsync(
                j => j.Id == jobOpeningId,
                j => j,
                "CreatedBy",
                "JobCandidates.Candidate.User"
            );

            if (jobOpening == null)
                return NotFound("Job opening not found");

            // Ensure recruiter owns the job opening
            if (jobOpening.CreatedBy.UserId != userId)
                return Forbid();

            var offeredCandidates = jobOpening.JobCandidates
                .Where(c => c.Status == "OfferSent")
                .Select(c => new OfferPoolDto
                {
                    JobCandidateId = c.Id,
                    jobOpeningId = c.JobOpeningId,
                    candidateId = c.CandidateId,
                    UserId = c.Candidate.UserId,
                    CandidateName = c.Candidate.User.FullName,
                    Email = c.Candidate.User.Email,
                    Status = c.Status,
                    OfferExpiryDate = c.OfferExpiryDate,
                })
                .ToList();

            return Ok(offeredCandidates);
        }


        [HttpPut("reject-by-system/{jobCandidateId}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> RejectOfferBySystem(
            int jobCandidateId,
            [FromBody] RejectOfferBySystemDto dto)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var jobCandidate = await _jobCandidateRepositoryCommon.GetWithIncludeAsync(
                    c => c.Id == jobCandidateId,
                    c => c,
                    "JobOpening.CreatedBy"
                );

                if (jobCandidate == null)
                    return NotFound("Job candidate not found");

                if (jobCandidate.JobOpening.CreatedBy.UserId != userId)
                    return Forbid("You are not authorized for this job opening");

                if (jobCandidate.Status != "OfferSent")
                    return BadRequest("Only active offers can be rejected");

                jobCandidate.Status = "OfferRejectedBySystem";
                jobCandidate.OfferRejectionReason = dto.Reason;
                jobCandidate.UpdatedAt = DateTime.UtcNow;

                await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

                return Ok(new
                {
                    message = "Offer rejected by system successfully"
                });
            }

        [HttpPut("{jobCandidateId}/extend-expiry")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> ExtendOfferExpiry(
           int jobCandidateId,
           [FromBody] ExtendOfferDto dto)
        {
            if (dto.NewExpiryDate <= DateTime.UtcNow)
                return BadRequest("Expiry date must be in the future");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var jobCandidate = await _jobCandidateRepositoryCommon.GetWithIncludeAsync(
                c => c.Id == jobCandidateId,
                c => c,
                "JobOpening.CreatedBy"
            );

            if (jobCandidate == null)
                return NotFound("Job candidate not found");

            if (jobCandidate.JobOpening.CreatedBy.UserId != userId)
                return Forbid("You are not authorized for this job opening");

            if (jobCandidate.Status != "OfferSent")
                return BadRequest("Offer expiry can only be extended for active offers");

            jobCandidate.OfferExpiryDate = dto.NewExpiryDate;
            jobCandidate.UpdatedAt = DateTime.UtcNow;

            await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

            return Ok(new
            {
                message = "Offer expiry date extended successfully",
                newExpiryDate = dto.NewExpiryDate
            });
        }

    }
}