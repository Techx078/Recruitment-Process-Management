using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WebApis.Data;
using WebApis.Dtos.JobOpeningDto;
using WebApis.Repository;
using WebApis.Repository.JobOpeningRepository;
using WebApis.Service.ErrorHandlingService;
using WebApis.Service.ErrroHandlingService;
using WebApis.Service.ValidationService;

namespace WebApis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobOpeningController : Controller
    {
        private readonly ICommonRepository<Recruiter> _recruiterRepository;
        private readonly ICommonRepository<JobOpening> _jobOpeningRepository;
        private readonly ICommonRepository<JobReviewer> _jobReviewerRepository;
        private readonly ICommonRepository<JobInterviewer> _jobInterviewerRepository;
        private readonly ICommonRepository<JobDocument> _jobDocumentRepository;
        private readonly ICommonRepository<JobSkill> _jobSkillRepository;
        private readonly ICommonRepository<Skill> _skillRepository;
        private readonly ICommonValidator<JobOpeningDto> _jobOpeningDtoValidator;
        private readonly IJobOpeningRepository _JobOpeningRepository;
        private readonly ICommonValidator<JobOpeningUpdateDto> _jobOpeningUpdateDtoValidator;
        private readonly ICommonRepository<Reviewer> _reviewerRepository;
        private readonly ICommonRepository<Interviewer> _interviewerRepository;
        private readonly ICommonRepository<Document> _documentRepository;

        public JobOpeningController(
            ICommonRepository<Document> documentRepository,
            ICommonRepository<Interviewer> interviewerRepository,
            ICommonRepository<Reviewer> reviewerRepository,
            ICommonRepository<Recruiter> recruiterRepository,
            ICommonRepository<JobOpening> jobOpeningRepository,
            ICommonRepository<JobReviewer> jobReviewerRepository,
            ICommonRepository<JobDocument> jobDocumentRepository,
            ICommonRepository<JobInterviewer> jobInterviewerRepository,
            ICommonRepository<JobSkill> jobSkillRepository,
            ICommonRepository<Skill> skillRepository,
            ICommonValidator<JobOpeningDto> jobOpeningDtoValidator,
            IJobOpeningRepository JobOpeningRepository,
            ICommonValidator<JobOpeningUpdateDto> jobOpeningUpdateValidator
        )
        {
            _documentRepository = documentRepository;
            _interviewerRepository = interviewerRepository;
            _recruiterRepository = recruiterRepository;
            _jobOpeningRepository = jobOpeningRepository;
            _jobReviewerRepository = jobReviewerRepository;
            _jobInterviewerRepository = jobInterviewerRepository;
            _jobDocumentRepository = jobDocumentRepository;
            _jobSkillRepository = jobSkillRepository;
            _skillRepository = skillRepository;
            _jobOpeningDtoValidator = jobOpeningDtoValidator;
            _JobOpeningRepository = JobOpeningRepository;
            _jobOpeningUpdateDtoValidator = jobOpeningUpdateValidator;
            _reviewerRepository = reviewerRepository;
        }

        //create job listing with linked reviewers, interviewers, and documents
        [HttpPost("create")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CreateJobOpening([FromBody] JobOpeningDto dto)
        {
            
            var validation = await _jobOpeningDtoValidator.ValidateAsync(dto);

            if (!validation.IsValid)
                throw new AppException(
                   "Validation failed",
                   ErrorCodes.ValidationError,
                   StatusCodes.Status400BadRequest,
                   validation.Errors
               );

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
               throw new UnauthorizedAccessException("Invalid token");

            int recruiterUserId = int.Parse(userIdClaim);

            // Validate Recruiter exists
            var recruiter = await _recruiterRepository.GetByFilterAsync(r => r.UserId == recruiterUserId);
            
            if (recruiter == null)
                throw new AppException(
                     "Recruiter does not exist",
                     ErrorCodes.NotFound,
                     StatusCodes.Status404NotFound
                 );

            //  Create JobOpening entity
            var job = new JobOpening
            {
                Title = dto.Title,
                Description = dto.Description,
                // Serialize list properties to JSON strings
                Requirement = dto.Requirement != null
                    ? JsonSerializer.Serialize(dto.Requirement)
                    : null,
                Responsibilities = dto.Responsibilities != null
                    ? JsonSerializer.Serialize(dto.Responsibilities)
                    : null,
                Benefits = dto.Benefits != null
                    ? JsonSerializer.Serialize(dto.Benefits)
                    : null,

                SalaryRange = dto.SalaryRange,
                minDomainExperience = dto.minDomainExperience,
                Domain = dto.Domain.ToString(),
                DeadLine = dto.DeadLine,
                Location = dto.Location.ToString(),
                Department = dto.Department.ToString(),
                JobType = dto.JobType.ToString(),
                Education = dto.Education.ToString(),
                Status = dto.Status.ToString(),
                CreatedById = recruiter.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _jobOpeningRepository.AddAsync(job);

            await _JobOpeningRepository.AddReviewersAsync(job.Id, dto.ReviewerIds);
            await _JobOpeningRepository.AddInterviewersAsync(job.Id, dto.InterviewerIds);
            await _JobOpeningRepository.AddDocumentsAsync(job.Id, dto.Documents);
            await _JobOpeningRepository.AddJobSkillsAsync(job.Id, dto.JobSkills);

            return Ok(new
            {

                message = "Job opening created successfully with all linked entities.",
                job.Id
            });
        }

        ////get all job listings with linked reviewers, interviewers, and documents
        [HttpGet("list")]
        public async Task<IActionResult> GetAllJobOpenings()
        {

            var jobs = await _jobOpeningRepository.GetAllByFilterAsync(
             j => true,
             j => new showJobOpeningDto
             {
                 Id = j.Id,
                 Title = j.Title,
                 Description = j.Description,
                 Requirement = j.Requirement,
                 SalaryRange = j.SalaryRange,
                 Benefits = j.Benefits,
                 Education = j.Education,   
                 Location = j.Location,
                 Department = j.Department,
                 JobType = j.JobType,
                 MinDomainExperience = j.minDomainExperience,
                 Domain = j.Domain,
                 Status = j.Status,
                 DeadLine = j.DeadLine,
                 CreatedAt = j.CreatedAt,

                 Recruiter = j.CreatedBy == null ? null : new RecruiterDto
                 {
                     Id = j.CreatedBy.Id,
                     UserId = j.CreatedBy.UserId
                 },

                 Reviewers = j.JobReviewers.Select(r => new ReviewerDto
                 {
                     Id = r.Reviewer.Id,
                     Email = r.Reviewer.User != null ? r.Reviewer.User.Email : null
                 }).ToList(),

                 Interviewers = j.JobInterviewers.Select(i => new InterviewerDto
                 {
                     Id = i.Interviewer.Id,
                     Email = i.Interviewer.User != null ? i.Interviewer.User.Email : null
                 }).ToList(),

                 Documents = j.JobDocuments.Select(d => new DocumentDto
                 {
                     Id = d.Document.id,
                     Name = d.Document.Name,
                     Description = d.Document.Description,
                     IsMandatory = d.IsMandatory
                 }).ToList(),

                 JobSkills = j.JobSkills.Select(s => new JobSkillDto
                 {
                     SkillId = s.Skill.SkillId,
                     Name = s.Skill.Name,
                     IsRequired = s.IsRequired,
                     MinExperience = s.minExperience
                 }).ToList()
             });
       
            if (jobs == null)
            {
                throw new AppException(
                     "job not found !",
                     ErrorCodes.NotFound,
                     StatusCodes.Status404NotFound
                 );
            }

            return Ok(jobs);
        }


        ////get specific by id job listing with linked reviewers, interviewers, and documents
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetJobOpeningById(int id)
        {
            var jobOpening = await _jobOpeningRepository.GetByFilterAsync(
             j => j.Id == id,
             j => new showJobOpeningDto
             {
                 Id = j.Id,
                 Title = j.Title,
                 Description = j.Description,
                 Requirement = j.Requirement,
                 SalaryRange = j.SalaryRange,
                 Benefits = j.Benefits,
                 Education = j.Education,
                 Location = j.Location,
                 Department = j.Department,
                 JobType = j.JobType,
                 MinDomainExperience = j.minDomainExperience,
                 Domain = j.Domain,
                 Status = j.Status,
                 DeadLine = j.DeadLine,
                 CreatedAt = j.CreatedAt,
                Responsibilities =  j.Responsibilities,
                 Recruiter = j.CreatedBy == null ? null : new RecruiterDto
                 {
                     Id = j.CreatedBy.Id,
                     UserId = j.CreatedBy.UserId
                 },

                 Reviewers = j.JobReviewers.Select(r => new ReviewerDto
                 {
                     Id = r.Reviewer.Id,
                     Email = r.Reviewer.User != null ? r.Reviewer.User.Email : null
                 }).ToList(),

                 Interviewers = j.JobInterviewers.Select(i => new InterviewerDto
                 {
                     Id = i.Interviewer.Id,
                     Email = i.Interviewer.User != null ? i.Interviewer.User.Email : null
                 }).ToList(),

                 Documents = j.JobDocuments.Select(d => new DocumentDto
                 {
                     JobDocumentId = d.Id,
                     Id = d.Document.id,
                     Name = d.Document.Name,
                     Description = d.Document.Description,
                     IsMandatory = d.IsMandatory
                 }).ToList(),

                 JobSkills = j.JobSkills.Select(s => new JobSkillDto
                 {
                     SkillId = s.Skill.SkillId,
                     Name = s.Skill.Name,
                     IsRequired = s.IsRequired,
                     MinExperience = s.minExperience
                 }).ToList()
             });

            if (jobOpening == null)
                throw new AppException(
                    "Job opening not found",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );

            return Ok(jobOpening);
        }

        //update fields by recruiter only
        [HttpPut("{id}/fields")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJobOpeningFields(int id, [FromBody] JobOpeningUpdateDto dto)
        {

            var validation = await _jobOpeningUpdateDtoValidator.ValidateAsync(dto);

            if (!validation.IsValid)
                throw new AppException(
                  "Validation failed",
                  ErrorCodes.ValidationError,
                  StatusCodes.Status400BadRequest,
                  validation.Errors
                );
            var job = await _jobOpeningRepository.GetByFilterAsync(j => j.Id == id);
            if (job == null)
                throw new AppException("Job not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Invalid token");

            int userId = int.Parse(userIdClaim);

            var recruiter = await _recruiterRepository.GetByFilterAsync(r => r.UserId == userId);
            
            if (recruiter == null || job.CreatedById != recruiter.Id)
                throw new AppException(
                   "You are not authorized to update this job",
                   ErrorCodes.Forbidden,
                   StatusCodes.Status403Forbidden
               );

            job.Title = dto.Title;
            job.Description = dto.Description;
            job.SalaryRange = dto.SalaryRange;
            job.Location = dto.Location.ToString();
            job.Department = dto.Department.ToString();
            job.JobType = dto.JobType.ToString();
            job.Education = dto.Education.ToString();
            job.Status = dto.Status.ToString();
            job.minDomainExperience = dto.minDomainExperience;
            job.Domain = dto.Domain.ToString();
            job.DeadLine = dto.DeadLine;

            job.Responsibilities = dto.Responsibilities != null
                ? JsonSerializer.Serialize(dto.Responsibilities)
                : null;

            job.Requirement = dto.Requirement != null
                ? JsonSerializer.Serialize(dto.Requirement)
                : null;

            job.Benefits = dto.Benefits != null
                ? JsonSerializer.Serialize(dto.Benefits)
                : null;

            // Update timestamp
            job.UpdatedAt = DateTime.UtcNow;

            await _jobOpeningRepository.UpdateAsync(job);

            return Ok(new { message = "Job fields updated successfully." });
        }
      
        ////update job reviewers linked to a job listing
        [HttpPatch("update-reviewers/{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJobReviewers(int id, [FromBody] List<int> reviewerIds)
        {
            if (id <= 0)
                throw new AppException("Invalid Job ID.", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            if (reviewerIds == null)
                throw new AppException("Reviewer cannot be empty !", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            if (!reviewerIds.Any())
                throw new AppException("At least slect one reviewer", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            if (reviewerIds.Count != reviewerIds.Distinct().Count())
                throw new AppException("Dublicate reviewr not allowed", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            var job = await _jobOpeningRepository.GetWithIncludeAsync(
                j => j.Id == id,
                j => j,
                "JobReviewers");
           
            if (job == null)
                throw new AppException("Job not found.", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Invalid token");

            int userId = int.Parse(userIdClaim);

            var recruiter = await _recruiterRepository.GetByFilterAsync(r => r.UserId == userId);

            if (recruiter == null || job.CreatedById != recruiter.Id)
                throw new AppException("You are not authorized to update this job.",
                   ErrorCodes.Forbidden,
                   StatusCodes.Status403Forbidden);

            // Check reviewers exist
            var existingReviewers = await _reviewerRepository.GetAllByFilterAsync(
                r => reviewerIds.Contains(r.Id),
                r => r.Id);
         
            if (existingReviewers.Count != reviewerIds.Count)
                throw new AppException("Some reviewer IDs do not exist.",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest);

            // Replace old reviewers
            await _jobReviewerRepository.RemoveRangeAsync(job.JobReviewers);
            job.JobReviewers = reviewerIds.Select(rid => new JobReviewer
            {
                ReviewerId = rid,
                JobOpeningId = job.Id
            }).ToList();

            await _jobOpeningRepository.SaveChangesAsync();

            return Ok(new { message = "Reviewers updated successfully." });
        }

        ////update job interviewers linked to a job listing
        [HttpPatch("update-interviewers/{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJobInterviewers(int id, [FromBody] List<int> interviewerIds)
        {
            if (id <= 0)
                throw new AppException("Invalid Job ID.", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            if (interviewerIds == null)
                throw new AppException("Interviewer list cannot be null.", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            if (!interviewerIds.Any())
                throw new AppException("At least one interviewer must be selected.", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            if (interviewerIds.Count != interviewerIds.Distinct().Count())
                throw new AppException("Duplicate interviewer IDs are not allowed.", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            var job = await _jobOpeningRepository.GetWithIncludeAsync(
               j => j.Id == id,
               j => j,
               "JobInterviewers");

            if (job == null)
                throw new AppException("Job not found.", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Invalid token");

            int userId = int.Parse(userIdClaim);

            var recruiter = await _recruiterRepository.GetByFilterAsync(r => r.UserId == userId);

            if (recruiter == null || job.CreatedById != recruiter.Id)
                throw new AppException("You are not authorized to update this job.",
                ErrorCodes.Forbidden,
                StatusCodes.Status403Forbidden);

            var existingInterviewers = await _interviewerRepository.GetAllByFilterAsync(
                    i => interviewerIds.Contains(i.Id),
                    i => i.Id );
            if (existingInterviewers.Count != interviewerIds.Count)
                throw new AppException("Some interviewer do not exist.",
                   ErrorCodes.ValidationError,
                   StatusCodes.Status400BadRequest);

            await _jobInterviewerRepository.RemoveRangeAsync(job.JobInterviewers);
            job.JobInterviewers = interviewerIds.Select(iid => new JobInterviewer
            {
                InterviewerId = iid,
                JobOpeningId = job.Id
            }).ToList();

            await _jobOpeningRepository.SaveChangesAsync();
          
            return Ok(new { message = "Interviewers updated successfully." });
        }

        //update job documents linked to a job listing
        [HttpPatch("update-documents/{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJobDocuments(int id, [FromBody] List<jobDocumentDto> documents)
        {
            var job = await _jobOpeningRepository.GetWithIncludeAsync(
                j => j.Id == id,
                j => j,
                "JobDocuments");
            if (job == null)
                throw new AppException("Job not found.", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Invalid token");

            int userId = int.Parse(userIdClaim);

            var recruiter = await _recruiterRepository.GetByFilterAsync(r => r.UserId == userId);

            if (recruiter == null || job.CreatedById != recruiter.Id)
                throw new AppException("You are not authorized to update this job.",
                   ErrorCodes.Forbidden,
                   StatusCodes.Status403Forbidden);

            var documentIds = documents.Select(d => d.DocumentId).ToList();

            var existingDocs = await _documentRepository.GetAllByFilterAsync(
                d => documentIds.Contains(d.id),
                d => d.id);
           
            if (existingDocs.Count != documentIds.Count)
                throw new AppException("Some document IDs do not exist.",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest);   

            await _jobDocumentRepository.RemoveRangeAsync(job.JobDocuments);
            job.JobDocuments = documents.Select(doc => new JobDocument
            {
                JobOpeningId = job.Id,
                DocumentId = doc.DocumentId,
                IsMandatory = doc.IsMandatory
            }).ToList();

            await _jobOpeningRepository.SaveChangesAsync();

            return Ok(new { message = "Documents updated successfully." });
        }

        //update job skills linked to a job listing
        [HttpPatch("update-skills/{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJobSkills(
            int id,
            [FromBody] List<jobSkillDto> skills)
        {
            if (skills == null || !skills.Any())
                throw new AppException("At least one skill is required.", ErrorCodes.ValidationError, StatusCodes.Status400BadRequest);

            var job = await _jobOpeningRepository.GetWithIncludeAsync(
                j => j.Id == id,
                j => j,
                "JobSkills.Skill");

            if (job == null)
                throw new AppException("Job not found.", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new  UnauthorizedAccessException("Invalid token.");

            int userId = int.Parse(userIdClaim);

            var recruiter = await _recruiterRepository.GetByFilterAsync(r => r.UserId == userId);
            if (recruiter == null || job.CreatedById != recruiter.Id)
                throw new AppException("You are not authorized to update this job.", ErrorCodes.Forbidden, StatusCodes.Status403Forbidden);

            var requestedSkillNames = skills
                .Select(s => s.SkillName.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existingSkills = await _skillRepository.GetAllByFilterAsync(
                s => requestedSkillNames
                    .Select(n => n.ToLower())
                    .Contains(s.Name.ToLower()),
                s => s
            );

            var existingSkillNames = existingSkills
                .Select(s => s.Name.ToLower())
                .ToHashSet();

            var newSkills = requestedSkillNames
                .Where(name => !existingSkillNames.Contains(name.ToLower()))
                .Select(name => new Skill
                {
                    Name = name
                })
                .ToList();

            if (newSkills.Any())
            {
                await _skillRepository.AddRangeAsync(newSkills);
                existingSkills.AddRange(newSkills);
            }

            await _jobSkillRepository.RemoveRangeAsync(job.JobSkills);

            job.JobSkills = skills.Select(s =>
            {
                var skill = existingSkills.First(es =>
                    es.Name.Equals(s.SkillName, StringComparison.OrdinalIgnoreCase));

                return new JobSkill
                {
                    JobOpeningId = job.Id,
                    SkillId = skill.SkillId,
                    IsRequired = s.IsRequired,
                    minExperience = s.minExperience
                };
            }).ToList();

            await _jobOpeningRepository.SaveChangesAsync();

            return Ok(new { message = "Skills updated successfully." });
        }

        //delete job listing and all linked reviewers, interviewers, and documents
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> DeleteJobOpening(int id)
        {
            var jobOpening = await _jobOpeningRepository.GetWithIncludeAsync(
                j => j.Id == id,
                j => j,
                "JobReviewers",
                "JobInterviewers",
                "JobDocuments",
                "JobSkills"
            );

            if (jobOpening == null)
                throw new AppException("Job not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Invalid token");

            int userId = int.Parse(userIdClaim);

            var recruiter = await _recruiterRepository.GetByFilterAsync(r => r.UserId == userId);

            if (recruiter == null || jobOpening.CreatedById != recruiter.Id)
                throw new AppException(
                    "You are not authorized to delete this job",
                    ErrorCodes.Forbidden,
                    StatusCodes.Status403Forbidden
                );

            // Delete linked entries first (junction tables)
            await _jobReviewerRepository.RemoveRangeAsync(jobOpening.JobReviewers);
            await _jobInterviewerRepository.RemoveRangeAsync(jobOpening.JobInterviewers);
            await _jobDocumentRepository.RemoveRangeAsync(jobOpening.JobDocuments);
            await _jobSkillRepository.RemoveRangeAsync(jobOpening.JobSkills);
            
            // Then delete main job record
            await _jobOpeningRepository.DeleteAsync(jobOpening);
            return Ok(new { message = "Job opening and related data deleted successfully." });
        }

    }
}
