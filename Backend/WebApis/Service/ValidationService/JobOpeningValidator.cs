using Microsoft.EntityFrameworkCore;
using WebApis.Data;
using WebApis.Dtos.JobOpeningDto;

namespace WebApis.Service.ValidationService
{
    public class JobOpeningValidator : ICommonValidator<JobOpeningDto>
    {
        private readonly AppDbContext _db;

        public JobOpeningValidator(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ValidationResult> ValidateAsync(JobOpeningDto dto)
        {
            var result = new ValidationResult();

            // Null Check
            if (dto == null)
            {
                result.Errors.Add("Request body is null.");
                return result;
            }
            if( dto.minDomainExperience < 0)
            {
                result.Errors.Add("Domain experience should be in positive ");
                return result;
            }
            // Basic Rules
            if (dto.DeadLine <= DateTime.UtcNow)
                result.Errors.Add("Deadline must be in the future.");

            if (dto.ReviewerIds == null || !dto.ReviewerIds.Any())
                result.Errors.Add("At least 1 reviewer required.");

            if (dto.InterviewerIds == null || !dto.InterviewerIds.Any())
                result.Errors.Add("At least 1 interviewer required.");

            if (!Enum.IsDefined(dto.JobType))
                result.Errors.Add("Invalid job type.");

            if (!Enum.IsDefined(dto.Department))
                result.Errors.Add("Invalid department.");

            if (!Enum.IsDefined(dto.Status))
                result.Errors.Add("Invalid job status.");

            // Documents (DTO rules)
            if (dto.Documents != null)
            {
                foreach (var doc in dto.Documents)
                {
                    if (doc.DocumentId <= 0)
                        result.Errors.Add("DocumentId must be greater than 0.");
                }
            }

            // Skills
            if (dto.JobSkills != null)
            {
                foreach (var s in dto.JobSkills)
                {
                    if (string.IsNullOrWhiteSpace(s.SkillName))
                        result.Errors.Add("Skill name cannot be empty.");

                    if (s.minExperience < 0)
                        result.Errors.Add("Min experience must be ≥ 0.");
                }

                if (dto.JobSkills
                      .GroupBy(x => x.SkillName.ToLower().Trim())
                      .Any(g => g.Count() > 1))
                {
                    result.Errors.Add("Duplicate skills are not allowed.");
                }
            }

           
            // Reviewers
            if (dto.ReviewerIds?.Any() == true)
            {
                var existingReviewerIds = await _db.Reviewers
                    .Where(r => dto.ReviewerIds.Contains(r.Id))
                    .Select(r => r.Id)
                    .ToListAsync();

                if (existingReviewerIds.Count != dto.ReviewerIds.Count)
                    result.Errors.Add("Some reviewer IDs do not exist.");
            }

            // Interviewers
            if (dto.InterviewerIds?.Any() == true)
            {
                var existingInterviewerIds = await _db.Interviewers
                    .Where(i => dto.InterviewerIds.Contains(i.Id))
                    .Select(i => i.Id)
                    .ToListAsync();

                if (existingInterviewerIds.Count != dto.InterviewerIds.Count)
                    result.Errors.Add("Some interviewer IDs do not exist.");
            }

            // Documents (DB check)
            if (dto.Documents?.Any() == true)
            {
                var documentIds = dto.Documents
                    .Select(d => d.DocumentId)
                    .ToList();

                var existingDocumentIds = await _db.Documents
                    .Where(d => documentIds.Contains(d.id))
                    .Select(d => d.id)
                    .ToListAsync();

                if (existingDocumentIds.Count != documentIds.Count)
                    result.Errors.Add("Some document IDs do not exist.");
            }

            return result;
        }
    }
}
