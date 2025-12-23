namespace WebApis.Dtos.JobOpeningDto
{
    public class showJobOpeningDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Requirement { get; set; }
        public string SalaryRange { get; set; }
        public string Benefits { get; set; }
        public string Education { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string JobType { get; set; }
        public int? MinDomainExperience { get; set; }
        public string Domain { get; set; }
        public string Status { get; set; }
        public DateTime? DeadLine { get; set; }
        public DateTime CreatedAt { get; set; }

        public RecruiterDto? Recruiter { get; set; }
        public List<ReviewerDto> Reviewers { get; set; } = new();
        public List<InterviewerDto> Interviewers { get; set; } = new();
        public List<DocumentDto> Documents { get; set; } = new();
        public List<JobSkillDto> JobSkills { get; set; } = new();
    }

    public class RecruiterDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }

    public class ReviewerDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
    }

    public class InterviewerDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
    }

    public class DocumentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class JobSkillDto
    {
        public int SkillId { get; set; }
        public string Name { get; set; }
        public bool IsRequired { get; set; }
        public int? MinExperience { get; set; }
    }

}
