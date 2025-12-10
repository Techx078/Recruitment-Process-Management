using Microsoft.AspNetCore.Mvc;
using WebApis.Data;
using WebApis.Dtos.JobCandidateDtos;

namespace WebApis.Repository.JobCandidateRepository
{
    public class JobCandidateRepository : IJobCandidateRepository
    {
        private readonly AppDbContext _db;

        public JobCandidateRepository(AppDbContext db) {
            _db = db;
        }

        public async Task<JobCandidate> CreateJobCandidate(JobCandidateCreateDto dto)
        {
            var JobCandidate = new JobCandidate
            {
                JobOpeningId = dto.JobOpeningId,
                CandidateId = dto.CandidateId,
                CvPath = dto.CvPath,
                Status = Status.Applied.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _db.JobCandidate.AddAsync(JobCandidate);
            await _db.SaveChangesAsync();
            return JobCandidate;
        }
    }
}
