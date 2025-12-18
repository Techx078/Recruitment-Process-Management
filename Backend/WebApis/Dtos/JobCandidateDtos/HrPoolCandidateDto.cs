namespace WebApis.Dtos.JobCandidateDtos
{
    public class HrPoolCandidateDto
    {
        public int JobCandidateId { get; set; }

        public int CandidateId { get; set; }
        public int UserId { get; set; }
        public string CandidateName { get; set; }
        public string Email { get; set; }
        public int RoundCompleted { get; set; }
        public string JobTitle { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
