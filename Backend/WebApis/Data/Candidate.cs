namespace WebApis.Data
{
    public class Candidate
    {
        public int Id { get; set; } 
        public string Education { get; set; }
        public string LinkedInProfile { get; set; }
        public string GitHubProfile { get; set; }
        public string ResumePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
