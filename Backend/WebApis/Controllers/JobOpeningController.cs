using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos;

namespace WebApis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobOpeningController : Controller
    {
        private readonly AppDbContext _db;

        public JobOpeningController(AppDbContext db)
        {
            _db = db;
        }

        //create job listing with linked reviewers, interviewers, and documents
        //Pending:-link cadidate to job opening
        [HttpPost("create")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CreateJobOpening(JobOpeningDto dto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            int RecruiterId = int.Parse(userIdClaim);

            //Validate Recruiter exists
            var Recruiter = await _db.Recruiter
               .Where(u => u.UserId == RecruiterId)
               .FirstOrDefaultAsync();
          
            if (Recruiter == null)
            {
                return BadRequest(new { message = "Recruiter does not exist." });
            }
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

            //  Validate Documents
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
            var job = new JobOpening
            {
                Title = dto.Title,
                Description = dto.Description,
                Requirnment = dto.Requirement,
                SalryRange = dto.SalaryRange,
                Benefits = dto.Benefits,
                DeadLine = dto.DeadLine,
                Status = "Open",
                CreatedById = Recruiter.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.JobOpening.Add(job);
            await _db.SaveChangesAsync();

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

            //  Link existing documents to job
            if (dto.Documents != null)
            {
                foreach (var doc in dto.Documents)
                {
                    _db.JobDocuments.Add(new JobDocument
                    {
                        JobOpeningId = job.Id,
                        DocumentId = doc.DocumentId ,
                        IsMandatory = doc.IsMandatory
                    });
                }
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Job opening created successfully with linked reviewers, interviewers, and documents.",
                jobId = job.Id
            });
        }

        //get all job listings with counts of linked reviewers, interviewers, and documents
        //pending:- link cadidate to job opening retrieval
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
                    j.Requirnment,
                    j.SalryRange,
                    j.Benefits,
                    j.Status,
                    j.DeadLine,
                    j.CreatedAt,
                    Recruiter = new
                    {
                        j.CreatedBy.Id,
                    },
                    Reviewers = j.JobReviewers.Select(r => new
                    {
                        r.Reviewer.Id,
                        r.Reviewer.User.Email,
                    }).ToList(),
                    Interviewers = j.JobInterviewers.Select(i => new
                    {
                        i.Interviewer.Id,
                        i.Interviewer.User.Email,
                    }).ToList(),
                    Documents = j.JobDocuments.Select(d => new
                    {
                        d.Document.id,
                        d.Document.Name,
                        d.Document.Description,
                        d.IsMandatory
                    }).ToList()
                })
                .ToListAsync();

            return Ok(jobs);
        }

        //update specific field of a job listing
        [HttpPatch("update/{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJobField(int id, [FromBody] Dictionary<string, string> update)
        {
            var job = await _db.JobOpening.FindAsync(id);
            if (job == null)
                return NotFound(new { message = "Job not found." });

            var field = update.Keys.FirstOrDefault();
            var value = update.Values.FirstOrDefault();

            if (string.IsNullOrEmpty(field))
                return BadRequest(new { message = "No field provided." });

            switch (field.ToLower())
            {
                case "title":
                    job.Title = value;
                    break;
                case "description":
                    job.Description = value;
                    break;
                case "status":
                    job.Status = value;
                    break;
                case "salaryrange":
                    job.SalryRange = value;
                    break;
                case "benefits":
                    job.Benefits = value;
                    break;
                default:
                    return BadRequest(new { message = "Unsupported field update." });
            }

            job.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { message = $"Job field '{field}' updated successfully." });
        }

        //update job reviewers linked to a job listing
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

        //update job interviewers linked to a job listing
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


        //delete job listing and all linked reviewers, interviewers, and documents
        //pending :- link Jobcandidate to job opening deletion
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> DeleteJobOpening(int id)
        {
            var job = await _db.JobOpening
                .Include(j => j.JobReviewers)
                .Include(j => j.JobInterviewers)
                .Include(j => j.JobDocuments)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                return NotFound(new { message = "Job not found." });

            // Delete linked entries first (junction tables)
            _db.JobReviewer.RemoveRange(job.JobReviewers);
            _db.jobInterviewer.RemoveRange(job.JobInterviewers);
            _db.JobDocuments.RemoveRange(job.JobDocuments);

            // Then delete main job record
            _db.JobOpening.Remove(job);

            await _db.SaveChangesAsync();

            return Ok(new { message = "Job opening and related data deleted successfully." });
        }

    }
}
