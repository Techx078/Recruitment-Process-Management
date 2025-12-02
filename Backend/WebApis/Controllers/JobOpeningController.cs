using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WebApis.Data;
using WebApis.Dtos.JobOpeningDto;
using WebApis.Repository;

namespace WebApis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobOpeningController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ICommonRepository<Recruiter> _recruiterRepository;
        private readonly ICommonRepository<JobOpening> _jobOpeningRepository;
        private readonly ICommonRepository<JobReviewer> _jobReviewerRepository;
        private readonly ICommonRepository<JobInterviewer> _jobInterviewerRepository;
        private readonly ICommonRepository<JobDocument> _jobDocumentRepository;
        private readonly ICommonRepository<JobSkill> _jobSkillRepository;
        private readonly ICommonRepository<Skill> _skillRepository;

        public JobOpeningController(
            AppDbContext db,
            ICommonRepository<Recruiter> recruiterRepository,
            ICommonRepository<JobOpening> jobOpeningRepository,
            ICommonRepository<JobReviewer> jobReviewerRepository,
            ICommonRepository<JobDocument> jobDocumentRepository,
            ICommonRepository<JobInterviewer> jobInterviewerRepository,
            ICommonRepository<JobSkill> jobSkillRepository,
            ICommonRepository<Skill> skillRepository
        )
        {
            _db = db;
            _recruiterRepository = recruiterRepository;
            _jobOpeningRepository = jobOpeningRepository;
            _jobReviewerRepository = jobReviewerRepository;
            _jobInterviewerRepository = jobInterviewerRepository;
            _jobDocumentRepository = jobDocumentRepository;
            _jobSkillRepository = jobSkillRepository;
            _skillRepository = skillRepository;
        }

        //create job listing with linked reviewers, interviewers, and documents
        //Pending:-link cadidate to job opening
        [HttpPost("create")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CreateJobOpening([FromBody] JobOpeningDto dto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized(new { message = "User is not authorized." });

            int recruiterUserId = int.Parse(userIdClaim);

            // Validate Recruiter exists
            var recruiter = await _recruiterRepository.GetByFilterAsync(r => r.UserId == recruiterUserId);
            
            if (recruiter == null)
                return BadRequest(new { message = "Recruiter does not exist." });

            // Validate Reviewers
            if (dto.ReviewerIds != null && dto.ReviewerIds.Any())
            {
                var existingReviewerIds = await _db.Reviewers
                    .Where(r => dto.ReviewerIds.Contains(r.Id))
                    .Select(r => r.Id)
                    .ToListAsync();

                if (existingReviewerIds.Count != dto.ReviewerIds.Count)
                    return BadRequest(new { message = "Some reviewer IDs do not exist." });
            }

            // Validate Interviewers
            if (dto.InterviewerIds != null && dto.InterviewerIds.Any())
            {
                var existingInterviewerIds = await _db.Interviewers
                    .Where(i => dto.InterviewerIds.Contains(i.Id))
                    .Select(i => i.Id)
                    .ToListAsync();

                if (existingInterviewerIds.Count != dto.InterviewerIds.Count)
                    return BadRequest(new { message = "Some interviewer IDs do not exist." });
            }

            // Validate Documents
            if (dto.Documents != null && dto.Documents.Any())
            {
                var documentIds = dto.Documents.Select(d => d.DocumentId).ToList();
                var existingDocumentIds = await _db.Documents
                    .Where(d => documentIds.Contains(d.id))
                    .Select(d => d.id)
                    .ToListAsync();

                if (existingDocumentIds.Count != documentIds.Count)
                    return BadRequest(new { message = "Some document IDs do not exist." });
            }

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
                Experience = dto.Experience,
                DeadLine = dto.DeadLine,
                Location = dto.Location,
                Department = dto.Department,
                JobType = dto.JobType,
                Education = dto.Education,
                Status = dto.Status,
                CreatedById = recruiter.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };


            await _jobOpeningRepository.AddAsync(job);
           
            // Add Reviewers
            if (dto.ReviewerIds != null)
            {
                foreach (var reviewerId in dto.ReviewerIds)
                {
                    _db.JobReviewer.Add(new JobReviewer
                    {
                        JobOpeningId = job.Id,
                        ReviewerId = reviewerId
                    });
                }
            }

            //  Add Interviewers
            if (dto.InterviewerIds != null)
            {
                foreach (var interviewerId in dto.InterviewerIds)
                {
                    _db.jobInterviewer.Add(new JobInterviewer
                    {
                        JobOpeningId = job.Id,
                        InterviewerId = interviewerId
                    });
                }
            }

            //  Add Documents
            if (dto.Documents != null)
            {
                foreach (var doc in dto.Documents)
                {
                    _db.JobDocuments.Add(new JobDocument
                    {
                        JobOpeningId = job.Id,
                        DocumentId = doc.DocumentId,
                        IsMandatory = doc.IsMandatory
                    });
                }
            }

            // Add Skills (Required / Preferred)
            if (dto.JobSkills != null)
            {
                foreach (var skill in dto.JobSkills)
                {
                    var skillTemp = await _skillRepository.GetByFilterAsync(s => s.Name.ToLower() == skill.SkillName.ToLower());
                   
                    if(skillTemp == null)
                    {
                        // Create new Skill if it doesn't exist
                        skillTemp = new Skill
                        {
                            Name = skill.SkillName
                        };
                        await _skillRepository.AddAsync(skillTemp);
                        
                    }
                    _db.jobSkills.Add(new JobSkill
                    {
                        JobOpeningId = job.Id,
                        SkillId = skillTemp.SkillId,
                        IsRequired = skill.IsRequired
                    });
                }
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Job opening created successfully with all linked entities.",
                jobId = job.Id
            });
        }


        ////get all job listings with linked reviewers, interviewers, and documents
        ////pending:- link cadidate to job opening retrieval
        [HttpGet("list")]
        [Authorize]
        public async Task<IActionResult> GetAllJobOpenings()
        {
            var jobs = await _db.JobOpening
                .Include(j => j.JobReviewers)
                    .ThenInclude(jr => jr.Reviewer)
                    .ThenInclude(r => r.User)
                .Include(j => j.JobInterviewers)
                    .ThenInclude(ji => ji.Interviewer)
                    .ThenInclude(i => i.User)
                .Include(j => j.JobDocuments)
                    .ThenInclude(jd => jd.Document)
                .Include(j => j.CreatedBy)
                .Select(j => new
                {
                    j.Id,
                    j.Title,
                    j.Description,
                    j.Requirement,       
                    j.SalaryRange,      
                    j.Benefits,
                    j.Education,
                    j.Location,
                    j.Department,
                    j.JobType,
                    j.Experience,
                    j.Status,
                    j.DeadLine,
                    j.CreatedAt,

                    Recruiter = j.CreatedBy == null ? null : new
                    {
                        j.CreatedBy.Id,
                        j.CreatedBy.UserId
                    },

                    Reviewers = j.JobReviewers.Select(r => new
                    {
                        r.Reviewer.Id,
                        Email = r.Reviewer.User != null ? r.Reviewer.User.Email : null
                    }),

                    Interviewers = j.JobInterviewers.Select(i => new
                    {
                        i.Interviewer.Id,
                        Email = i.Interviewer.User != null ? i.Interviewer.User.Email : null
                    }),

                    Documents = j.JobDocuments.Select(d => new
                    {
                        d.Document.id,       
                        d.Document.Name,
                        d.Document.Description,
                        d.IsMandatory
                    }),

                    JobSkills = j.JobSkills.Select(s => new
                    {
                        s.Skill.SkillId,
                        s.Skill.Name,
                        s.IsRequired
                    })
                })
                .ToListAsync();

            return Ok(jobs);
        }


        ////get specific by id job listing with linked reviewers, interviewers, and documents
        ////pending:- link cadidate to job opening retrieval
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetJobOpeningById(int id)
        {
            var jobOpening = await _db.JobOpening
             .Where(j => j.Id == id)
             .Include(j => j.JobReviewers)
                 .ThenInclude(jr => jr.Reviewer)
                 .ThenInclude(r => r.User)
             .Include(j => j.JobInterviewers)
                 .ThenInclude(ji => ji.Interviewer)
                 .ThenInclude(i => i.User)
             .Include(j => j.JobDocuments)
                 .ThenInclude(jd => jd.Document)
             .Include(j => j.CreatedBy)
             .Select(j => new
             {
                 j.Id,
                 j.Title,
                 j.Description,
                 j.Requirement,
                 j.SalaryRange,
                 j.Benefits,
                 j.Education,
                 j.Responsibilities,
                 j.Location,
                 j.Department,
                 j.JobType,
                 j.Experience,
                 j.Status,
                 j.DeadLine,
                 j.CreatedAt,

                 Recruiter = j.CreatedBy == null ? null : new
                 {
                     j.CreatedBy.Id,
                     j.CreatedBy.UserId
                 },

                 Reviewers = j.JobReviewers.Select(r => new
                 {
                     r.Reviewer.Id,
                     Email = r.Reviewer.User != null ? r.Reviewer.User.Email : null
                 }),

                 Interviewers = j.JobInterviewers.Select(i => new
                 {
                     i.Interviewer.Id,
                     Email = i.Interviewer.User != null ? i.Interviewer.User.Email : null
                 }),

                 Documents = j.JobDocuments.Select(d => new
                 {
                     d.Document.id,
                     d.Document.Name,
                     d.Document.Description,
                     d.IsMandatory
                 }),

                 JobSkills = j.JobSkills.Select(s => new
                 {
                     s.Skill.SkillId,
                     s.Skill.Name,
                     s.IsRequired
                 })
             })
             .FirstOrDefaultAsync();
            return Ok(jobOpening);
        }


        //update fields by recruiter only
        [HttpPut("{id}/fields")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJobOpeningFields(int id, [FromBody] JobOpeningDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var job = await _jobOpeningRepository.GetByFilterAsync(j => j.Id == id);
            if (job == null)
                return NotFound(new { message = "Job not found." });

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized(new { message = "Invalid token." });

            int userId = int.Parse(userIdClaim);

            var recruiter = await _recruiterRepository.GetByFilterAsync(r => r.UserId == userId);
            
            if (recruiter == null || job.CreatedById != recruiter.Id)
            {
                return BadRequest(new { message = "You are not authorized to update this job." });
            }

            job.Title = dto.Title;
            job.Description = dto.Description;
            job.SalaryRange = dto.SalaryRange;
            job.Location = dto.Location;
            job.Department = dto.Department;
            job.JobType = dto.JobType;
            job.Education = dto.Education;
            job.Status = dto.Status;
            job.Experience = dto.Experience;
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

            // Save
            await _db.SaveChangesAsync();

            return Ok(new { message = "Job fields updated successfully." });
        }

        ////update specific field of a job listing
        [HttpPatch("update-field/{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJobField(int id, [FromBody] Dictionary<string, object> update)
        {
            var job = await _jobOpeningRepository.GetByFilterAsync(j => j.Id == id);
          
            if (job == null)
                return NotFound(new { message = "Job not found." });

            var field = update.Keys.FirstOrDefault();
            var value = update.Values.FirstOrDefault();

            if (string.IsNullOrEmpty(field))
                return BadRequest(new { message = "No field provided." });


            switch (field.ToLower())
            {
                case "title":
                    job.Title = value?.ToString();
                    break;

                case "description":
                    job.Description = value?.ToString();
                    break;

                case "salaryrange":
                    job.SalaryRange = value?.ToString();
                    break;

                case "status":

                    if (!int.TryParse(value?.ToString(), out int statusValue))
                        return BadRequest(new { message = "Status must be a valid numeric value." });
                    if (!Enum.IsDefined(typeof(JobStatus), statusValue))
                        return BadRequest(new { message = $"Invalid status value." });
                    job.Status = (JobStatus)statusValue;
                    break;

                case "location":
                    if (!int.TryParse(value?.ToString(), out int locationValue))
                        return BadRequest(new { message = "Location must be a valid numeric value." });

                    if (!Enum.IsDefined(typeof(JobLocation), locationValue))
                        return BadRequest(new { message = $"Invalid location value)"});
                    job.Location = (JobLocation)locationValue;
                    break;

                case "department":
                    if (!int.TryParse(value?.ToString(), out int departmentValue))
                        return BadRequest(new { message = "Department must be a valid numeric value." });

                    if (!Enum.IsDefined(typeof(Department), departmentValue))
                        return BadRequest(new { message = $"Invalid department value" });
                    job.Department = (Department)departmentValue;
                    break;

                case "education":
                    if (!int.TryParse(value?.ToString(), out int educationValue))
                        return BadRequest(new { message = "Education must be a valid numeric value." });

                    if (!Enum.IsDefined(typeof(EducationLevel), educationValue))
                        return BadRequest(new { message = $"Invalid education value" });
                    job.Education = (EducationLevel)educationValue;
                    break;

                case "jobtype":
                    if(!int.TryParse(value?.ToString(), out int jobTypeValue))
                    return BadRequest(new { message = "JobType must be a valid numeric value." });

                    if (!Enum.IsDefined(typeof(JobType), jobTypeValue))
                        return BadRequest(new { message = $"Invalid job type value" });
                    job.JobType = (JobType)jobTypeValue;
                    break;

                //  Serialize list fields into JSON
                case "benefits":
                    job.Benefits =JsonSerializer.Serialize(value);
                    break;

                case "requirement":
                    job.Requirement = JsonSerializer.Serialize(value); 
                    break;
                case "responsibilities":
                    job.Responsibilities = JsonSerializer.Serialize(value);
                    break;
                default:
                    return BadRequest(new { message = $"Unsupported field update: {field}" });
            }

            job.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { message = $"Job field '{field}' updated successfully." });
        }

        ////update job reviewers linked to a job listing
        [HttpPatch("update-reviewers/{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJobReviewers(int id, [FromBody] List<int> reviewerIds)
        {
            var job = await _db.JobOpening
                .Include(j => j.JobReviewers)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                return NotFound(new { message = "Job not found." });

            // Check reviewers exist
            var existingReviewers = await _db.Reviewers
                .Where(r => reviewerIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToListAsync();

            if (existingReviewers.Count != reviewerIds.Count)
                return BadRequest(new { message = "Some reviewer IDs do not exist." });

            // Replace old reviewers
            _db.JobReviewer.RemoveRange(job.JobReviewers);
            job.JobReviewers = reviewerIds.Select(rid => new JobReviewer
            {
                ReviewerId = rid,
                JobOpeningId = job.Id
            }).ToList();

            await _db.SaveChangesAsync();

            return Ok(new { message = "Reviewers updated successfully." });
        }

        ////update job interviewers linked to a job listing
        [HttpPatch("update-interviewers/{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJobInterviewers(int id, [FromBody] List<int> interviewerIds)
        {
            var job = await _db.JobOpening
                .Include(j => j.JobInterviewers)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                return NotFound(new { message = "Job not found." });

            var existingInterviewers = await _db.Interviewers
                .Where(i => interviewerIds.Contains(i.Id))
                .Select(i => i.Id)
                .ToListAsync();

            if (existingInterviewers.Count != interviewerIds.Count)
                return BadRequest(new { message = "Some interviewer IDs do not exist." });

            _db.jobInterviewer.RemoveRange(job.JobInterviewers);
            job.JobInterviewers = interviewerIds.Select(iid => new JobInterviewer
            {
                InterviewerId = iid,
                JobOpeningId = job.Id
            }).ToList();

            await _db.SaveChangesAsync();

            return Ok(new { message = "Interviewers updated successfully." });
        }

        //update job documents linked to a job listing
        [HttpPatch("update-documents/{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJobDocuments(int id, [FromBody] List<DocumentDto> documents)
        {
            var job = await _db.JobOpening
                .Include(j => j.JobDocuments)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                return NotFound(new { message = "Job not found." });

            var documentIds = documents.Select(d => d.DocumentId).ToList();

            var existingDocs = await _db.Documents
                .Where(d => documentIds.Contains(d.id))
                .Select(d => d.id)
                .ToListAsync();

            if (existingDocs.Count != documentIds.Count)
                return BadRequest(new { message = "Some document IDs do not exist." });

            _db.JobDocuments.RemoveRange(job.JobDocuments);
            job.JobDocuments = documents.Select(doc => new JobDocument
            {
                JobOpeningId = job.Id,
                DocumentId = doc.DocumentId,
                IsMandatory = doc.IsMandatory
            }).ToList();

            await _db.SaveChangesAsync();

            return Ok(new { message = "Documents updated successfully." });
        }

        //update job skills linked to a job listing
        [HttpPatch("{id}/skills")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJobSkills(int id, [FromBody] List<SkillDto> skills)
        {
            var job = await _db.JobOpening
                .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                return NotFound(new { message = "Job not found." });

            // Extract skill names from the request
            var skillNames = skills.Select(s => s.SkillName.Trim().ToLower()).ToList();

            // Fetch existing skills from DB that match provided names
            var existingSkills = await _db.Skill
                .Where(s => skillNames.Contains(s.Name.ToLower()))
                .ToListAsync();

            // Validation: check if all provided skill names exist
            if (existingSkills.Count != skillNames.Count)
            {
                var existingNames = existingSkills.Select(s => s.Name.ToLower()).ToHashSet();
                var missingNames = skillNames.Where(s => !existingNames.Contains(s)).ToList();
                return BadRequest(new { message = $"These skills do not exist: {string.Join(", ", missingNames)}" });
            }

            // Remove old job-skill relationships
            _db.jobSkills.RemoveRange(job.JobSkills);

            // Add new job-skill relationships
            job.JobSkills = skills.Select(s => new JobSkill
            {
                JobOpeningId = job.Id,
                SkillId = existingSkills.First(es => es.Name.Equals(s.SkillName, StringComparison.OrdinalIgnoreCase)).SkillId,
                IsRequired = s.IsRequired
            }).ToList();

            await _db.SaveChangesAsync();

            return Ok(new { message = "Skills updated successfully." });
        }

        //delete job listing and all linked reviewers, interviewers, and documents
        //pending :- link Jobcandidate to job opening deletion
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> DeleteJobOpening(int id)
        {
            var jobOpening = await _db.JobOpening
                .Include(j => j.JobReviewers)
                .Include(j => j.JobInterviewers)
                .Include(j => j.JobDocuments)
                .Include(j => j.JobSkills)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jobOpening == null)
                return NotFound(new { message = "Job not found." });

            // Delete linked entries first (junction tables)
            _db.JobReviewer.RemoveRange(jobOpening.JobReviewers);
            _db.jobInterviewer.RemoveRange(jobOpening.JobInterviewers);
            _db.JobDocuments.RemoveRange(jobOpening.JobDocuments);
            _db.jobSkills.RemoveRange(jobOpening.JobSkills);

            // Then delete main job record
            await _jobOpeningRepository.DeleteAsync(jobOpening);
            return Ok(new { message = "Job opening and related data deleted successfully." });
        }

    }
}
