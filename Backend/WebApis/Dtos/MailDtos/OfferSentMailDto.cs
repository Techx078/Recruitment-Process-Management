namespace WebApis.Dtos.MailDtos
{
    public class OfferSentMailDto
    {
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }

        public string JobTitle { get; set; }
        public DateTime OfferExpiryDate { get; set; }
    }
}
