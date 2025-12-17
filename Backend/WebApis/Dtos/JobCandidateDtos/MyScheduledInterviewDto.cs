namespace WebApis.Dtos.JobCandidateDtos
{
    public class MyScheduledInterviewDto
    {
        public int JobInterviewId { get; set; }
        public int JobCandidateId { get; set; }

        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }

        public string InterviewType { get; set; }
        public int RoundNumber { get; set; }

        public DateTime ScheduledAt { get; set; }
        public string MeetingLink { get; set; }
    }
}
