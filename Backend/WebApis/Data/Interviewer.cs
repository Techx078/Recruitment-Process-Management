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
       public List<JobInterviewer> JobInterviewers { get; set; }
        public ICollection<JobInterview> JobInterviews { get; set; }

    }
}
