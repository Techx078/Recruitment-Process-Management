namespace WebApis.Data
{
    public class Recruiter
    {
        public int Id { get; set; }
        public string Department { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        //navigation for the JobOpenings
        public List<JobOpening> JobOpenings { get; set; }
    }
}
