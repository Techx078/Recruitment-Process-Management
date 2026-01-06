namespace WebApis.Data
{
    public class JobDocument
    {
        public int Id { get; set; }
        public int JobOpeningId { get; set; }
        public int DocumentId { get; set; }
        public bool IsMandatory { get; set; } = true;
        public Document Document { get; set; }
        public JobOpening JobOpening { get; set; }

        public ICollection<JobCandidateDocus> JobCandidateDoc { get; set; } = new List<JobCandidateDocus>();
    }
}
