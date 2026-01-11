namespace WebApis.Dtos.MailDtos
{
    public class OfferExpiryExtendedMailDto
    {
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }

        public string JobTitle { get; set; }
        public DateTime NewExpiryDate { get; set; }
    }
}
