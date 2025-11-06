using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApis.Data; // To access enums like JobType, Department, etc.

namespace WebApis.Dtos
{
    public class DocumentDto
    {
        [Required]
        public int DocumentId { get; set; }
        public bool IsMandatory { get; set; } = true;
    }

    public class SkillDto
    {
        [Required]
        public String SkillName { get; set; }

        // true = Required, false = Preferred
        public bool IsRequired { get; set; } = true;
    }

    public class JobOpeningDto
    {
        // --- Basic Info ---
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

       
        public string? SalaryRange { get; set; }

        [Required]
        public JobLocation Location { get; set; } = JobLocation.Remote;

        [Required]
        public Department Department { get; set; }

        [Required]
        public JobType JobType { get; set; } = JobType.FullTime;

        [Required]
        public EducationLevel Education { get; set; } = EducationLevel.Bachelors;

        [Required]
        public JobStatus Status { get; set; } = JobStatus.Open;

        public string? Experience { get; set; } = "0"; // e.g. "2–5 years"
        public List<string>? Responsibilities { get; set; }
        public List<string>? Requirement { get; set; }
        //store like string in database
        public List<string>? Benefits { get; set; }

        
        [Required]
        public DateTime DeadLine { get; set; }

        //Relationships 
        [Required]
        public List<int> ReviewerIds { get; set; } = new();

        [Required]
        public List<int> InterviewerIds { get; set; } = new();

        public List<DocumentDto>? Documents { get; set; } = new();

        public List<SkillDto>? JobSkills { get; set; } = new();
    }
}
