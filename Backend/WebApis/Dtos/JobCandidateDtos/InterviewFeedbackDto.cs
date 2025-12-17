using System.ComponentModel.DataAnnotations;

namespace WebApis.Dtos.JobCandidateDtos
{
    public class InterviewFeedbackDto
    {
        [Required]
        public bool IsPassed { get; set; }

        public int? Marks { get; set; }

        public string? Feedback { get; set; }

        // Only required if IsPassed = true
        public string? NextStep { get; set; } // Technical | HR | Finish
    }
}
