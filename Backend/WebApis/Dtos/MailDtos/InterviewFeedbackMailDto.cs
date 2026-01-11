namespace WebApis.Dtos.MailDtos
{
    public class InterviewFeedbackMailDto
    {
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }

        public string JobTitle { get; set; }
        public string InterviewType { get; set; }
        public int RoundNumber { get; set; }

        public bool IsPassed { get; set; }
        public string NextStep { get; set; }   // Technical / HR / Finish / Rejected
    }
}
