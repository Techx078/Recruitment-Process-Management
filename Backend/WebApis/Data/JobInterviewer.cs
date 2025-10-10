namespace WebApis.Data
{
    public class JobInterviewer
    {
        public int Id { get; set; }
        public int JobOpeningId { get; set; }
        public int InterviewerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        //Navigation properties
        public JobOpening JobOpening { get; set; }
        public Interviewer Interviewer { get; set; }
    }
}
