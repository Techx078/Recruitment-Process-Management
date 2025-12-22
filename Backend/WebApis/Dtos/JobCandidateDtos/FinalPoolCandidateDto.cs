namespace WebApis.Dtos.JobCandidateDtos
{
    public class FinalPoolCandidateDto
    {
        public int JobCandidateId { get; set; }
        public string CandidateName { get; set; }
        public string Email { get; set; }
        public int TotalRounds { get; set; }
        public string LastInterviewType { get; set; }
        public DateTime? LastInterviewDate { get; set; }
        public string Status { get; set; }
    }
}
