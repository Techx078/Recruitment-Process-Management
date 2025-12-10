using Microsoft.AspNetCore.Mvc;
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
