using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos.JobCandidateDtos;
using WebApis.Repository.JobCandidateRepository;
using WebApis.Service.ValidationService;
using WebApis.Service.ValidationService.JobCandidateValidator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace WebApis.Controllers.JobCandidateController
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobCandidateController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IJobCandidateRepository _jobCandidateRepository;
        private readonly ICommonValidator<JobCandidateCreateDto> _jobCandidateCreateValidator;
        public JobCandidateController(AppDbContext db,
            IJobCandidateRepository jobCandidateRepository,
            ICommonValidator<JobCandidateCreateDto> jobCandidateCreateValidator
        )
        {
            _db = db;
            _jobCandidateRepository = jobCandidateRepository;
            _jobCandidateCreateValidator = jobCandidateCreateValidator;
        }
        [HttpPost("create")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CreateJobCandidate([FromBody] JobCandidateCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Request body cannot be null.");
            }
            //Validation can be added here
            var result = await _jobCandidateCreateValidator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized(new { message = "Invalid User." });

            int userId = int.Parse(userIdClaim);

            bool jobOpening = await _db.JobOpening
                               .AnyAsync(j => j.Id == dto.JobOpeningId && j.CreatedBy.UserId == userId);

            if (!jobOpening)
                return Forbid("You are not authorized to add candidates to this job opening.");

            //call repository method to create jobCandidate
            JobCandidate JobCandidate = await _jobCandidateRepository.CreateJobCandidate(dto);

            return CreatedAtAction(nameof(CreateJobCandidate),
            new { id = JobCandidate.Id },
            new
            {
                JobCandidate.Id,
                JobCandidate.JobOpeningId,
                JobCandidate.CandidateId,
                JobCandidate.CvPath,
                JobCandidate.Status,
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
                return Unauthorized(new { message = "Invalid User." });
            int userId = int.Parse(userIdClaim);

            bool jobOpening = await _db.JobOpening
                               .AnyAsync(j => j.Id == dto.JobOpeningId && j.CreatedBy.UserId == userId);

            if (!jobOpening)
                return Forbid("You are not authorized to add candidates to this job opening.");
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
                    continue; //skip invalid entries
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
        }
    }

