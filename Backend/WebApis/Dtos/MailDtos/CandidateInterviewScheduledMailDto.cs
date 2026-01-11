namespace WebApis.Dtos.MailDtos
{
    public class CandidateInterviewScheduledMailDto
    {
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }

        public string JobTitle { get; set; }
        public string InterviewType { get; set; }  
        public int RoundNumber { get; set; }

        public DateTime InterviewDate { get; set; }
        public string MeetingLink { get; set; }

        public string InterviewerName { get; set; }
    }

}
