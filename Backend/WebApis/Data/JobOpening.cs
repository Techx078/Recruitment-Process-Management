namespace WebApis.Data
{
    public class JobOpening
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Requirnment { get; set; }
        public string? SalryRange { get; set; }
        public string? Benefits { get; set; }
        public DateTime? DeadLine { get; set; }
        public string Status { get; set; } = "Open"; // Open, Closed, OnHold
        public string? StatusReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedById { get; set; }

        public Recruiter CreatedBy { get; set; }

        public ICollection<JobReviewer> JobReviewers { get; set; } = new List<JobReviewer>();
        public ICollection<JobInterviewer> JobInterviewers { get; set; } = new List<JobInterviewer>();

        public ICollection<JobCandidate> JobCandidates { get; set; } = new List<JobCandidate>();

    }
}
