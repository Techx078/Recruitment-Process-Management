using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Dtos.JobCandidateDtos;
using WebApis.Repository;
using WebApis.Repository.JobCandidateRepository;
using WebApis.Service;
using WebApis.Service.EmailService;
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
        private readonly ICommonRepository<JobCandidateDocus> _jobCandidateDocRepository;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ICommonRepository<JobDocument> _jobDocumentRepository;
        private readonly IAppEmailService _appEmailService;
        private readonly ICommonRepository<Employee> _employeeRepository;

        public JobCandidateController(
            IJobCandidateRepository jobCandidateRepository,
            ICommonValidator<JobCandidateCreateDto> jobCandidateCreateValidator,
            ICommonRepository<JobOpening> jobOpeningRepository,
            ICommonRepository<JobCandidate> jobCandidateRepositoryCommon,
            ICommonRepository<Reviewer> reviewerRepository,
            ICommonRepository<Interviewer> interviewerRepository,
            ICommonRepository<JobInterview> jobInterviewRepository,
            ICommonRepository<JobCandidateDocus> jobCandidateDocRepository,
            CloudinaryService cloudinaryService,
            ICommonRepository<JobDocument> jobDocumentRepository,
            IAppEmailService appEmailService,
            ICommonRepository<Employee> employeeRepository
        )
        {
            _jobCandidateRepository = jobCandidateRepository;
            _jobCandidateCreateValidator = jobCandidateCreateValidator;
            _jobOpeningRepository = jobOpeningRepository;
            _jobCandidateRepositoryCommon = jobCandidateRepositoryCommon;
            _reviewerRepository = reviewerRepository;
            _interviewerRepository = interviewerRepository;
            _jobInterviewRepository = jobInterviewRepository;
            _jobCandidateDocRepository = jobCandidateDocRepository;
            _cloudinaryService = cloudinaryService;
            _jobDocumentRepository = jobDocumentRepository;
            _appEmailService = appEmailService;
            _employeeRepository = employeeRepository;
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

            // Send email
            var mailData = await _jobCandidateRepository
                .GetCandidateJobMailData(jobCandidate.Id);

            await _appEmailService
                .SendCandidateAddedToJobEmailAsync(mailData);

            return CreatedAtAction(nameof(CreateJobCandidate),
                new { id = jobCandidate.Id },
                new
                {
                    jobCandidate.Id,
                    jobCandidate.JobOpeningId,
                    jobCandidate.CandidateId,
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

                    var jobCandidate = await _jobCandidateRepository.CreateJobCandidate(jobCandidateDto);
                    // Send email
                    var mailData = await _jobCandidateRepository
                        .GetCandidateJobMailData(jobCandidate.Id);

                    await _appEmailService
                        .SendCandidateAddedToJobEmailAsync(mailData);

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
        [Authorize(Roles = "Recruiter,Reviewer,Admin,Interviewer,Candidate")]
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

            //send mail to candidate about result
            var mailData = await _jobCandidateRepository
                .GetCandidateReviewMailData(jobCandidate.Id);

            await _appEmailService
                .SendCandidateReviewResultEmailAsync(mailData);

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

            //send e-mail to candidate about meeting information
            var mailData = await _jobCandidateRepository
                .GetCandidateInterviewScheduledMailData(interview.Id);

            await _appEmailService
                .SendInterviewScheduledEmailAsync(mailData);

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

            var mailData = await _jobCandidateRepository
                .GetInterviewFeedbackMailData(jobCandidate.Id);

            await _appEmailService
                .SendInterviewFeedbackResultEmailAsync(mailData);

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
                            ? $"Result: {(interview.IsPassed == true ? "Passed" : "Rejected")} marks : {(interview.Marks != 0 ? interview.Marks : 0)} Comment : {(interview.Feedback)}"
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

            var mailData =
                await _jobCandidateRepository.GetOfferSentMailData(jobCandidate.Id);

            await _appEmailService.SendOfferEmailAsync(mailData);

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
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> GetOfferedCandidates(int jobOpeningId)
        {
            var userRoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var jobOpening = await _jobOpeningRepository.GetWithIncludeAsync(
                  j => j.Id == jobOpeningId,
                  j => j,
                  "CreatedBy",
                  "JobCandidates.Candidate.User"
              );

            if (jobOpening == null)
                throw new KeyNotFoundException("Job opening not found");

            if (userRoleClaim == "Recruiter")
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userIdClaim))
                    throw new UnauthorizedAccessException("Invalid token");

                int userId = int.Parse(userIdClaim);
                
                // Ensure recruiter owns the job opening
                if (jobOpening.CreatedBy.UserId != userId)
                    throw new AppException(
                       "You are not authorized to view offered candidates",
                       ErrorCodes.Forbidden,
                       StatusCodes.Status403Forbidden
                   );
            }
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
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new UnauthorizedAccessException("Invalid token");

            int userId = int.Parse(userIdClaim);

            var jobCandidate = await _jobCandidateRepositoryCommon.GetWithIncludeAsync(
                    c => c.Id == jobCandidateId,
                    c => c,
                    "JobOpening.CreatedBy"
                );

                if (jobCandidate == null)
                    throw new KeyNotFoundException("Job candidate not found");

                if (jobCandidate.JobOpening.CreatedBy.UserId != userId)
                    throw new AppException(
                        "You are not authorized for this job opening",
                        ErrorCodes.Forbidden,
                        StatusCodes.Status403Forbidden
                    );

                if (jobCandidate.Status != "OfferSent")
                    throw new AppException(
                        "Only active offers can be rejected",
                        ErrorCodes.ValidationError,
                        StatusCodes.Status400BadRequest
                    );

                jobCandidate.Status = "OfferRejectedBySystem";
                jobCandidate.OfferRejectionReason = dto.Reason;
                jobCandidate.UpdatedAt = DateTime.UtcNow;

                await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

                var mailData =
                    await _jobCandidateRepository
                        .GetOfferRejectedBySystemMailData(jobCandidate.Id);

                await _appEmailService
                    .SendOfferRejectedBySystemEmailAsync(mailData);

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
                throw new AppException(
                    "Expiry date must be in the future",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new UnauthorizedAccessException("Invalid token");

            int userId = int.Parse(userIdClaim);

            var jobCandidate = await _jobCandidateRepositoryCommon.GetWithIncludeAsync(
                c => c.Id == jobCandidateId,
                c => c,
                "JobOpening.CreatedBy"
            );

            if (jobCandidate == null)
                throw new KeyNotFoundException("Job candidate not found");

            if (jobCandidate.JobOpening.CreatedBy.UserId != userId)
                throw new AppException(
                    "You are not authorized for this job opening",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            if (jobCandidate.Status != "OfferSent")
                throw new AppException(
                    "Offer expiry can only be extended for active offers",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            jobCandidate.OfferExpiryDate = dto.NewExpiryDate;
            jobCandidate.UpdatedAt = DateTime.UtcNow;

            await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

            var mailData =
                await _jobCandidateRepository
                    .GetOfferExpiryExtendedMailData(jobCandidate.Id);

            await _appEmailService
                .SendOfferExpiryExtendedEmailAsync(mailData);

            return Ok(new
            {
                message = "Offer expiry date extended successfully",
                newExpiryDate = dto.NewExpiryDate
            });
        }

        [HttpPost("{jobCandidateId}/documents/{jobDocumentId}")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> UploadJobCandidateDocument(
            int jobCandidateId,
            int jobDocumentId,
            IFormFile file)
        {
            if (file == null)
                throw new AppException("File is required", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new AppException("Invalid token", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            int loggedInUserId = int.Parse(userIdClaim);

            var jobCandidate = await _jobCandidateRepositoryCommon
                .GetWithIncludeAsync(
                    jc => jc.Id == jobCandidateId,
                    jc => jc,
                    "Candidate"
                );

            if (jobCandidate == null)
                throw new KeyNotFoundException("Job candidate not found");

            if (jobCandidate.Candidate.UserId != loggedInUserId)
                throw new AppException("You are not allowed to upload documents for this candidate", ErrorCodes.Forbidden, StatusCodes.Status403Forbidden);

            var jobDocument = await _jobDocumentRepository.GetWithIncludeAsync(
                jd => jd.Id == jobDocumentId,
                jd => jd,
                "JobOpening"
            );

            if (jobDocument == null)
                throw new KeyNotFoundException("Invalid job document");

            if (jobDocument.JobOpeningId != jobCandidate.JobOpeningId)
                throw new AppException("This document is not required for the applied job", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            var documentUrl = await _cloudinaryService
                .UploadJobCandidateDocumentAsync(file, jobCandidateId, jobDocumentId);

            var existingDoc = await _jobCandidateDocRepository
                .GetByFilterAsync(d =>
                    d.JobCandidateId == jobCandidateId &&
                    d.JobDocumentId == jobDocumentId
                );

            if (existingDoc == null)
            {
                var newDoc = new JobCandidateDocus
                {
                    JobCandidateId = jobCandidateId,
                    JobDocumentId = jobDocumentId,
                    DocumentUrl = documentUrl,
                    UploadedAt = DateTime.UtcNow
                };

                await _jobCandidateDocRepository.AddAsync(newDoc);
            }
            else
            {
                existingDoc.DocumentUrl = documentUrl;
                existingDoc.UploadedAt = DateTime.UtcNow;
                await _jobCandidateDocRepository.UpdateAsync(existingDoc);
            }

            return Ok(new
            {
                message = "Document uploaded successfully",
                documentUrl
            });
        }

        [HttpPut("{jobCandidateId}/documents/submit")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> SubmitAllDocuments(int jobCandidateId)
        {
            var jobCandidate = await _jobCandidateRepositoryCommon.GetWithIncludeAsync(
                jc => jc.Id == jobCandidateId,
                jc => jc,
                "JobOpening.JobDocuments",
                "JobCandidateDoc.JobDocument",
                "Candidate"
            );

            if (jobCandidate == null)
                throw new AppException(
                    "Job candidate not found",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            if( jobCandidate.Candidate.UserId != loggedInUserId)
            {
                throw new AppException(
                   "you are not able to perform this action",
                   ErrorCodes.Forbidden,
                   StatusCodes.Status403Forbidden
               );
            }
            if (jobCandidate.Status == "OfferAccepted" && jobCandidate.Status == "DocumentRejected")
                throw new AppException(
                    "No active offer accepted yet !",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );
            var requiredDocumentIds = jobCandidate.JobOpening.JobDocuments.Where( jd => jd.IsMandatory )
                .Select(jd => jd.DocumentId)
                .ToHashSet();

            var uploadedDocumentIds = jobCandidate.JobCandidateDoc
                ?.Where(d => d.DocumentUrl != null)
                .Select(d => d.JobDocument.DocumentId)
                .ToHashSet() ?? new HashSet<int>();

            var missingDocuments = requiredDocumentIds.Except(uploadedDocumentIds).ToList();

            if (missingDocuments.Any())
            {
                throw new AppException(
                    $"Missing required documents. Upload remaining documents before submitting.",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest,
                     uploadedDocumentIds.ToArray()
                );
            }

            jobCandidate.IsDocumentUploaded = true;
            jobCandidate.IsDocumentVerified = false;
            jobCandidate.DocumentUnVerificationReason = null;
            jobCandidate.UpdatedAt = DateTime.UtcNow;
            jobCandidate.Status = "DocumentUploaded";

            await _jobCandidateRepositoryCommon.SaveChangesAsync();

            return Ok(new
            {
                message = "All required documents uploaded successfully. Awaiting HR verification.",
            });
        }

        [HttpPut("{jobCandidateId}/documents/verify")]
        [Authorize(Roles = "Interviewer")]
        public async Task<IActionResult> VerifyCandidateDocuments(
            int jobCandidateId,
            [FromBody] VerifyDocumentsDto dto)
        {
            var jobCandidate = await _jobCandidateRepositoryCommon.GetWithIncludeAsync(
               jc => jc.Id == jobCandidateId,
               jc => jc,
               "JobOpening.JobInterviewers.Interviewer"
           );
            if (jobCandidate == null)
                throw new AppException(
                    "Job candidate not found",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            if (!jobCandidate.JobOpening.JobInterviewers.Any(ji =>
                ji.Interviewer.UserId == loggedInUserId &&
                ji.Interviewer.Department == "HR"))
            {
                throw new AppException(
                    "you are not authorized to update job-Opening",
                    ErrorCodes.Unauthorized,
                    StatusCodes.Status401Unauthorized
                );
            }

            if (!jobCandidate.IsDocumentUploaded)
                throw new AppException(
                    "Candidate has not submitted all required documents yet",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            if (dto.IsVerified)
            {
                jobCandidate.IsDocumentVerified = true;
                jobCandidate.DocumentUnVerificationReason = null;
                jobCandidate.Status = "DocumentsVerified"; 
                //pending :- pass candidate to employee
            }
            else
            {
                if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                    throw new AppException(
                        "Rejection reason is required when documents are rejected",
                        ErrorCodes.ValidationError,
                        StatusCodes.Status400BadRequest
                    );

                jobCandidate.IsDocumentVerified = false;
                jobCandidate.IsDocumentUploaded = false; // force re-upload
                jobCandidate.DocumentUnVerificationReason = dto.RejectionReason;
                jobCandidate.Status = "DocumentRejected";
            }

            jobCandidate.UpdatedAt = DateTime.UtcNow;

            await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

            var mailData =
                await _jobCandidateRepository
                    .GetCandidateDocumentVerificationMailData(jobCandidate.Id);

            await _appEmailService
                .SendCandidateDocumentVerificationEmailAsync(mailData);


            return Ok(new
            {
                message = dto.IsVerified
                    ? "Documents verified successfully"
                    : "Documents rejected. Candidate must re-upload documents."
            });
        }

        [HttpGet("{jobCandidateId}/documents")]
        [Authorize(Roles = "Interviewer")]
        public async Task<IActionResult> GetJobCandidateDocuments(int jobCandidateId)
        {
            var jobCandidate = await _jobCandidateRepositoryCommon
                .GetWithIncludeAsync(
                    jc => jc.Id == jobCandidateId,
                    jc => jc,
                    "JobCandidateDoc.JobDocument.Document",
                    "JobOpening.JobInterviewers.Interviewer"
                );

            if (jobCandidate == null)
                throw new AppException(
                    "Job candidate not found",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            if (!jobCandidate.JobOpening.JobInterviewers.Any(ji =>
                ji.Interviewer.UserId == loggedInUserId &&
                ji.Interviewer.Department == "HR"))
            {
                throw new AppException(
                    "you are not authorized to update job-Opening",
                    ErrorCodes.Unauthorized,
                    StatusCodes.Status401Unauthorized
                );
            }

            var documents = jobCandidate.JobCandidateDoc?
                .Select(d => new JobCandidateDocumentResponseDto
                {
                    JobCandidateDocumentId = d.Id,
                    DocumentId = d.JobDocumentId,
                    DocumentName = d.JobDocument.Document.Name,
                    DocumentDescription = d.JobDocument.Document.Description,
                    DocumentUrl = d.DocumentUrl,
                    UploadedAt = d.UploadedAt
                })
                .ToList() ?? new List<JobCandidateDocumentResponseDto>();

            return Ok(new
            {
                jobCandidateId = jobCandidate.Id,
                isDocumentUploaded = jobCandidate.IsDocumentUploaded,
                isDocumentVerified = jobCandidate.IsDocumentVerified,
                rejectionReason = jobCandidate.DocumentUnVerificationReason,
                documents
            });
        }

        [HttpGet("pool/documentUploaded/{jobOpeningId}")]
        [Authorize(Roles = "Interviewer")]
        public async Task<IActionResult> GetDocumentUploaded(int jobOpeningId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var hrId = await _interviewerRepository.GetByFilterAsync(
                   i => i.UserId == userId && i.Department == "HR",
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

            var jobOpening = await _jobOpeningRepository.GetWithIncludeAsync(
             j => j.Id == jobOpeningId,
             j => j,
             "JobCandidates.Candidate.User"
            );

            if (jobOpening == null)
                return NotFound("Job opening not found");

            var candidates = jobOpening.JobCandidates
                .Where(c => c.Status == "DocumentUploaded")
                .Select(c => new OfferPoolDto
                {
                    JobCandidateId = c.Id,
                    jobOpeningId = c.JobOpeningId,
                    candidateId = c.CandidateId,
                    UserId = c.Candidate.UserId,
                    CandidateName = c.Candidate.User.FullName,
                    Email = c.Candidate.User.Email,
                    Status = c.Status,
                })
                .ToList();

            return Ok(candidates);
        }

        [HttpGet("pool/PostOffer/{jobOpeningId}")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> GetPostOffer(int jobOpeningId)
        {
            var userRoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var jobOpening = await _jobOpeningRepository.GetWithIncludeAsync(
                   j => j.Id == jobOpeningId,
                   j => j,
                   "CreatedBy",
                   "JobCandidates.Candidate.User"
               );

            if (jobOpening == null)
                throw new KeyNotFoundException("Job opening not found");
            if (userRoleClaim == "Recruiter")
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userIdClaim))
                    throw new UnauthorizedAccessException("Invalid token");

                int userId = int.Parse(userIdClaim);

                // Ensure recruiter owns the job opening
                if (jobOpening.CreatedBy.UserId != userId)
                    throw new AppException(
                       "You are not authorized to view offered candidates",
                       ErrorCodes.Forbidden,
                       StatusCodes.Status403Forbidden
                   );
            }
            var offeredCandidates = jobOpening.JobCandidates
                .Where(c => c.Status == "OfferAccepted" || c.Status == "DocumentUploaded" || c.Status == "DocumentsVerified" || c.Status == "DocumentRejected"
                || c.Status == "OfferRejectedByCandidate" || c.Status == "OfferRejectedBySystem" || c.Status == "JoiningDateSend" || c.Status == "Employee")
                .Select(c => new OfferPoolDto
                {
                    JobCandidateId = c.Id,
                    jobOpeningId = c.JobOpeningId,
                    candidateId = c.CandidateId,
                    UserId = c.Candidate.UserId,
                    CandidateName = c.Candidate.User.FullName,
                    Email = c.Candidate.User.Email,
                    Status = c.Status
                })
                .ToList();

            return Ok(offeredCandidates);
        }

        [HttpPut("send-Joining-Date/{jobCandidateId}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> SendJoiningDate(
          int jobCandidateId,
          [FromBody] JoiningDateDto dto)
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

            if (jobCandidate.Status != "DocumentsVerified")
                throw new AppException(
                    "document should be verified to send joining date",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            if (jobCandidate.JobOpening.CreatedBy.UserId != userId)
                throw new AppException(
                    "You are not authorized to send offer for this candidate",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            if (dto == null)
                throw new AppException(
                    "Request body is required",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            if (dto.JoiningDate == default)
                throw new AppException(
                    "Joining date is required",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            var joiningDateUtc = dto.JoiningDate.ToUniversalTime();
            var todayUtc = DateTime.UtcNow.Date;

            if (joiningDateUtc.Date < todayUtc)
                throw new AppException(
                    "Joining date cannot be in the past",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            jobCandidate.JoiningDate = dto.JoiningDate;
            jobCandidate.Status = "JoiningDateSend";
            jobCandidate.UpdatedAt = DateTime.UtcNow;

            await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

            var mailData =await _jobCandidateRepository.GetCandidateJoiningDateMailData(jobCandidate.Id);

            await _appEmailService
                .SendCandidateJoiningDateEmailAsync(mailData);

            return Ok(new
            {
                message = "JoiningDate sent successfully"
            });
        }

        [HttpPost("Create-Employee/{jobCandidateId}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CreateEmployee(
          int jobCandidateId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException();

            int userId = int.Parse(userIdClaim);

            var jobCandidate = await _jobCandidateRepositoryCommon
                .GetWithIncludeAsync(
                    c => c.Id == jobCandidateId,
                    c => c,
                    "JobOpening.CreatedBy",
                    "Candidate.User",
                    "Candidate.User"
                );

            if (jobCandidate == null)
                throw new AppException(
                    "Job candidate not found",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            if (jobCandidate.Status != "JoiningDateSend")
                throw new AppException(
                    "joining date not sent!",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );

            if (jobCandidate.JobOpening.CreatedBy.UserId != userId)
                throw new AppException(
                    "You are not authorized to send offer for this candidate",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            await _employeeRepository.AddAsync(new Employee
            {
                UserId = jobCandidate.Candidate.UserId,
                Department = jobCandidate.JobOpening.Department,
                Designation = jobCandidate.JobOpening.Domain,
                salaryRange = jobCandidate.JobOpening.SalaryRange == null ? "not defined" : jobCandidate.JobOpening.SalaryRange,
                JobType = jobCandidate.JobOpening.JobType,
                Location = jobCandidate.JobOpening.Location,
                EmploymentStatus = "Active",
                JoiningDate = jobCandidate.JoiningDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            jobCandidate.Status = "Employee";
            await _jobCandidateRepositoryCommon.UpdateAsync(jobCandidate);

            var mailData =
                await _jobCandidateRepository
                    .GetEmployeeCreatedMailData(jobCandidate.Id);

            await _appEmailService
                .SendEmployeeWelcomeEmailAsync(mailData);

            return Ok(new
            {
                message = "Employee Created successfully"
            });
        }

    }
}