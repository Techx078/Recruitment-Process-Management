namespace WebApis.Data
{
    public class Reviewer
    {
        public int Id { get; set; }
        public string Department { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        //navigation with jobreviewer
        public ICollection<JobReviewer> JobReviewers { get; set; } = new List<JobReviewer>();
    }
}
