namespace WebApis.Dtos.JobCandidateDtos
{
    public class OfferPoolDto
    {
        public int JobCandidateId { get; set; }
        public int jobOpeningId { get; set; }
        public int candidateId { get; set; }

        public int UserId { get; set; }
        public string CandidateName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public DateTime? OfferExpiryDate { get; set; }
    }

}
