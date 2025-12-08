namespace WebApis.Data
{
    public class JobOpening
    {
        public int Id { get; set; }

        // Basic Details
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Requirement { get; set; }
        public string? SalaryRange { get; set; }
        // Enums
        public String Location { get; set; }
        public String Department { get; set; }
        public String JobType { get; set; }
        public string Education { get; set; }
        public string Status { get; set; } = "Open";

        // Additional Info
        public string Domain { get; set; } = string.Empty;
        public int minDomainExperience { get; set; } = 0;
        public string? Responsibilities { get; set; }
        public string? Benefits { get; set; } // JSON string or multiline text

        // Metadata
        public DateTime? DeadLine { get; set; }
        public string? StatusReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign Keys
        public int CreatedById { get; set; }
        public Recruiter CreatedBy { get; set; }

        // Navigation Collections
        public ICollection<JobReviewer> JobReviewers { get; set; } = new List<JobReviewer>();
        public ICollection<JobInterviewer> JobInterviewers { get; set; } = new List<JobInterviewer>();
        public ICollection<JobCandidate> JobCandidates { get; set; } = new List<JobCandidate>();
        public ICollection<JobDocument> JobDocuments { get; set; } = new List<JobDocument>();
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }
}
