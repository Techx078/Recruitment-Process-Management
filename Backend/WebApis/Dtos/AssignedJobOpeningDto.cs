namespace WebApis.Dtos
{
    public class AssignedJobOpeningDto
    {
        public int JobOpeningId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Department { get; set; }
        public string JobType { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CandidateCount { get; set; }
        public int? ReviewerCount { get; set; }
        public int? InterviewerCount { get; set; }
        public int? MinDomainExperience { get; set; }
        public string Domain { get; set; }
    }

}
