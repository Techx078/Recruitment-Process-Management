namespace WebApis.Dtos.MailDtos
{
    public class CandidateJoiningDateMailDto
    {
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }

        public string JobTitle { get; set; }
        public DateTime? JoiningDate { get; set; }
    }

}
