using WebApis.Dtos.JobOpeningDto;

namespace WebApis.Repository.JobOpeningRepository
{
    public interface IJobOpeningRepository
    {
        Task AddReviewersAsync(int jobId, IEnumerable<int> reviewerIds);
        Task AddInterviewersAsync(int jobId, IEnumerable<int> interviewerIds);
        Task AddDocumentsAsync(int jobId, IEnumerable<jobDocumentDto> documents);
        Task AddJobSkillsAsync(int jobId, IEnumerable<jobSkillDto> skills);
    }
}
