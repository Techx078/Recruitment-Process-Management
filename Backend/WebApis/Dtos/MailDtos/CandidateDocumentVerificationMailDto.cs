namespace WebApis.Dtos.MailDtos
{
    public class CandidateDocumentVerificationMailDto
    {
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }

        public string JobTitle { get; set; }
        public bool IsVerified { get; set; }
        public string? RejectionReason { get; set; }
    }
}
