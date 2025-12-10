namespace WebApis.Data
{
    public class JobInterview
    {
        public int Id { get; set; }

        public int JobCandidateId { get; set; }
        public JobCandidate JobCandidate { get; set; }

        public int InterviewerId { get; set; }
        public Interviewer Interviewer { get; set; }

        public int RoundNumber { get; set; }
        public string InterviewType { get; set; } // e.g. "Technical", "HR"
        public string MeetingLink { get; set; }
        public DateTime ScheduledAt { get; set; }

        public int? Marks { get; set; }
        public string? Feedback { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
