using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos.JobCandidateDtos;
using WebApis.Repository.JobCandidateRepository;
using WebApis.Service.ValidationService;
using WebApis.Service.ValidationService.JobCandidateValidator;

namespace WebApis.Controllers.JobCandidateController
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobCandidateController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IJobCandidateRepository _jobCandidateRepository;
        private readonly ICommonValidator<JobCandidateCreateDto> _jobCandidateCreateValidator;
        public JobCandidateController(AppDbContext db , 
            IJobCandidateRepository jobCandidateRepository ,
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
    }
}
