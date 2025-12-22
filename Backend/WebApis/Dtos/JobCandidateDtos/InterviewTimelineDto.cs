namespace WebApis.Dtos.JobCandidateDtos
{
    public class InterviewTimelineDto
    {
        public int JobCandidateId { get; set; }
        public string CandidateName { get; set; }
        public string JobTitle { get; set; }
        public string CurrentStatus { get; set; }

        public List<TimelineEventDto> Timeline { get; set; } = new();
    }
}
