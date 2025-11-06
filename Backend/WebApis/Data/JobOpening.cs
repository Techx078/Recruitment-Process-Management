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
        public JobLocation Location { get; set; }
        public Department Department { get; set; }
        public JobType JobType { get; set; }
        public EducationLevel Education { get; set; }
        public JobStatus Status { get; set; } = JobStatus.Open;

        // Additional Info
        public string? Experience { get; set; } // e.g. "2–5 years"
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
    public enum JobLocation
    {
        Remote = 1,
        OnSite = 2,
        Hybrid = 3
    }

    public enum Department
    {
        Engineering =1,
        HumanResources=2,
        Marketing=3,
        Sales=4,
        Finance=5,
        Operations=6,
        Design=7,
        ITSupport=8,
        ProductManagement=9
    }

    public enum JobType
    {
        FullTime=1,
        PartTime = 2,
        Contract = 3,
        Internship=4,
        Temporary=5
    }

    public enum EducationLevel
    {
        HighSchool=1,
        Diploma=2,
        Bachelors=3,
        Masters=4,
        Doctorate=5,
        Other=6
    }

    public enum JobStatus
    {
        Open=1,
        Closed=2,
        OnHold=3
    }
}
