namespace WebApis.Data
{
    public class JobCandidateDocus
    {
        public int Id { get; set; }
        public int JobCandidateId { get; set; }
        public JobCandidate JobCandidate { get; set; }
        public int JobDocumentId { get; set; }
        public JobDocument JobDocument { get; set; }
        public string DocumentUrl { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
