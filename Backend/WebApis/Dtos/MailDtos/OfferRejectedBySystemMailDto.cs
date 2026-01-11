namespace WebApis.Dtos.MailDtos
{
    public class OfferRejectedBySystemMailDto
    {
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }

        public string JobTitle { get; set; }
        public string RejectionReason { get; set; }
    }
}
