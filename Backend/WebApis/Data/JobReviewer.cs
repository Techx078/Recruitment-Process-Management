namespace WebApis.Data
{
    public class JobReviewer
    {
        public int Id { get; set; }

        // Foreign Keys
        public int JobOpeningId { get; set; }
        public int ReviewerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Reviewer Reviewer { get; set; } = null!;
        public JobOpening JobOpening { get; set; } = null!;
    }
}
