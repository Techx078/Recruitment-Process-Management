using System.ComponentModel.DataAnnotations;
namespace WebApis.Dtos
{
    public class DocumentDto{
        public int DocumentId { get; set; }
        public bool IsMandatory { get; set; } = true;
    }
    public class JobOpeningDto
    {        
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string? Requirement { get; set; }
        public string? SalaryRange { get; set; }
        public string? Benefits { get; set; }
        public DateTime DeadLine { get; set; }
        public List<int> ReviewerIds { get; set; }
        public List<int> InterviewerIds { get; set; }
        public List<DocumentDto> Documents { get; set; }
    }
}
