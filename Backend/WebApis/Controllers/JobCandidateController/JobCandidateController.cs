using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos.JobCandidateDtos;
using WebApis.Repository;
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
        private readonly IJobCandidateRepository _jobCandidateRepository;
        private readonly ICommonValidator<JobCandidateCreateDto> _jobCandidateCreateValidator;
        private readonly ICommonRepository<JobOpening> _jobOpeningRepository;
        public JobCandidateController(
            IJobCandidateRepository jobCandidateRepository,
            ICommonValidator<JobCandidateCreateDto> jobCandidateCreateValidator,
            ICommonRepository<JobOpening> jobOpeningRepository
        )
        {
            _jobCandidateRepository = jobCandidateRepository;
            _jobCandidateCreateValidator = jobCandidateCreateValidator;
            _jobOpeningRepository = jobOpeningRepository;
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
        }
    }

