namespace WebApis.Data
{
    public class JobCandidate
    {
        public int Id { get; set; }

        public int JobOpeningId { get; set; }
        public JobOpening JobOpening { get; set; }

        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; }

        public string CvPath { get; set; }
        public string Status { get; set; } // e.g. "Applied", "Reviewed", "Shortlisted", "Interviewing", "Rejected", "Selected"
        public string? ReviewerComment { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property to JobInterviews
        public ICollection<JobInterview> JobInterviews { get; set; }
    }
}
