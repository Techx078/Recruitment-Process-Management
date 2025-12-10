using System.ComponentModel.DataAnnotations;
using WebApis.Data;

namespace WebApis.Dtos.JobCandidateDtos
{
    public class JobCandidateCreateDto
    {
        [Required]
        public int JobOpeningId { get; set; }

        [Required]
        public int CandidateId { get; set; }
        [Required]
        public string CvPath { get; set; }
    }
    public enum Status
    {
        Applied = 1,
        Reviewed = 2,
        ScheduledInterview =3,
        WaitForInterView=4,
        Rejected=5,
        Shortlisted=6,
        Selected=7
    }
}
