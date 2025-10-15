namespace WebApis.Data
{
    public class Interviewer
    {
        public int Id { get; set; }
        public string Department { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }

        //navigation with user
        public User User { get; set; }
        //navigation with JobInterviewer
        public ICollection<JobInterviewer> JobInterviewers { get; set; } = new List<JobInterviewer>();
        public ICollection<JobInterview> JobInterviews { get; set; } = new List<JobInterview>();

    }
}
