using Microsoft.AspNetCore.Mvc;
using WebApis.Data;
using WebApis.Dtos.JobCandidateDtos;

namespace WebApis.Repository.JobCandidateRepository
{
    public interface IJobCandidateRepository
    {
        Task<JobCandidate> CreateJobCandidate(JobCandidateCreateDto dto);
    }
}
