namespace WebApis.Data
{
    public class JobCandidate
    {
        public int Id { get; set; }

        public int JobOpeningId { get; set; }
        public JobOpening JobOpening { get; set; }

        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; }

        public string CvPath { get; set; }
        public string Status { get; set; } // e.g. "Applied", "Reviewed", "ScheduledInterview", "WaitForInterview", "Rejected","Shortlisted" "Selected" , "offerSent" "pending", "OfferAccepted" "RejectedByCandidate" , "DocumentUploaded","DocumentsVerified","DocumentRejected"
        public string? ReviewerComment { get; set; }

        public int RoundNumber { get; set; } = 0;

        public int? ReviewerId {  get; set; }
        public Reviewer? Reviewer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsNextTechnicalRound { get; set; }
        public bool IsNextHrRound { get; set; }

        //document flow
        public bool IsDocumentUploaded { get; set; }

        public bool IsDocumentVerified { get; set ; }
        public string? DocumentUnVerificationReason { get; set; }

        //offer flow
        public DateTime OfferExpiryDate {  get; set; }

        public String? OfferRejectionReason { get; set; } = string.Empty;

        public DateTime? JoiningDate { get; set; } = DateTime.MinValue;

        // Navigation property to JobInterviews
        public ICollection<JobInterview>? JobInterviews { get; set; } = new List<JobInterview>();
        public ICollection<JobCandidateDocus> JobCandidateDoc { get; set; }
            = new List<JobCandidateDocus>();
    }
}
