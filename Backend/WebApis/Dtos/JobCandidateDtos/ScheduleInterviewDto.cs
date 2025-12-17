using System.ComponentModel.DataAnnotations;

namespace WebApis.Dtos.JobCandidateDtos
{
    public class ScheduleInterviewDto
    {
        [Required]
        public DateTime InterviewDate { get; set; }
        [Required]
        public string MeetingLink { get; set; }
    }
}
