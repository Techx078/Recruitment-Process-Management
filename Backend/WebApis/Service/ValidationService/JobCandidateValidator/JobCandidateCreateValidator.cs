using Microsoft.EntityFrameworkCore;
using WebApis.Data;
using WebApis.Dtos.JobCandidateDtos;

namespace WebApis.Service.ValidationService.JobCandidateValidator
{
    public class JobCandidateCreateValidator : ICommonValidator<JobCandidateCreateDto>
    {
        private readonly AppDbContext _db;

        public JobCandidateCreateValidator(AppDbContext db)
        {
            _db = db;
        }
        public async Task<ValidationResult> ValidateAsync(JobCandidateCreateDto dto)
        {
            ValidationResult result = new ValidationResult();

            // Null Check
            if (dto == null)
            {
                result.Errors.Add("Request body is null.");
                return result;
            }
            // JobOpeningId Check
            if (dto.JobOpeningId <= 0 
                || !await _db.JobOpening.AnyAsync(j => j.Id == dto.JobOpeningId))
            {
                result.Errors.Add("JobOpeningId is not valid");
            }
            // CandidateId Check
            if (dto.CandidateId <= 0 
                || !await _db.Candidates.AnyAsync(j => j.Id == dto.CandidateId))
            {
                result.Errors.Add("CandidateId is not valid ");
            }
            // CvPath Check
            if (string.IsNullOrWhiteSpace(dto.CvPath))
            {
                result.Errors.Add("CvPath cannot be null or empty.");
            }

            bool alreadyApplied = await _db.JobCandidate.AnyAsync(j =>
                                  j.JobOpeningId == dto.JobOpeningId &&
                                  j.CandidateId == dto.CandidateId);

            if (alreadyApplied)
            {
                result.Errors.Add("Candidate has already applied for this job.");
            }

            return result;
        }
    }
}
