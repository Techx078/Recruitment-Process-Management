namespace WebApis.Dtos.JobCandidateDtos
{
    public class TechnicalPoolCandidateDto
    {
        public int jobCandidateId { get; set; }
        public int jobOpeningId { get; set; }
        public string candidateName { get; set; }
        public string jobTitle { get; set; }
        public int roundNumber { get; set; }
        public int userId { get; set; }

        public int candidateId { get; set; }
        public DateTime lastUpdatedAt { get; set; }
    }
}
