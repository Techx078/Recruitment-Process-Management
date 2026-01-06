namespace WebApis.Dtos.JobCandidateDtos
{
    public class JobCandidateDocumentResponseDto
    {
        public int JobCandidateDocumentId { get; set; }
        public int DocumentId { get; set; }

        public string DocumentName { get; set; }
        public string? DocumentDescription { get; set; }

        public string DocumentUrl { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
