using Microsoft.AspNetCore.Mvc;
using WebApis.Data;
using WebApis.Dtos;

namespace WebApis.Repository.CandidateRepository
{
    public interface ICandidateRepository
    {
        Task<Candidate?> GetCandidateWithUserAsync(int userId);
        Task UpdateCandidateAsync(Candidate candidate, UpdateCandidateDto dto);

        Task<CandidateDto> GetCandidateDetailsByUserId(int UserId);

        Task<List<CandidateJobDto>> GetCnadidateJobOpeningDetailsByUserId(int UserId);
    }
}
