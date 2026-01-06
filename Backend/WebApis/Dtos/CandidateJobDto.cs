namespace WebApis.Dtos
{
    public class CandidateJobDto
    {
        public int Id { get; set; }                    
        public int JobOpeningId { get; set; } 
        
        public int candidateId { get; set; }

        // Basic JobOpening Info (safe to show candidate)
        public string JobTitle { get; set; }
        public string JobStatus { get; set; }
        // Application details
        public string CvPath { get; set; }
        public string Status { get; set; }              
        public int RoundNumber { get; set; }

        public string? DocumentUnVerificationReason {  get; set; } = string.Empty;
    }
}
