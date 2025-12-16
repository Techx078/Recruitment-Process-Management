namespace WebApis.Dtos.JobCandidateDtos
{
    public class PendingReviewCandidateDto
    {
        public int UserId { get; set; }

        public int JobOpeningId { get; set; }
        public int JobCandidateId { get; set; }
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }
        public string JobTitle { get; set; }
        public string CvPath { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
