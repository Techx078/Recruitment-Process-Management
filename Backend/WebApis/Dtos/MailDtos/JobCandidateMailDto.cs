namespace WebApis.Dtos.MailDtos
{
    public class JobCandidateMailDto
    {
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;
        public int JobOpeningId { get; set; }

        public string RecruiterName { get; set; } = string.Empty;
    }
}
