using System.ComponentModel.DataAnnotations;

namespace WebApis.Dtos.JobCandidateDtos
{
    public class JobCandidateCreateBulkDto
    {
        [Required]
        public int JobOpeningId { get; set; }
        [Required]
        public List<int> CandidateId { get; set; }
        [Required]
        public List<string> CvPath { get; set; }
    }
}
