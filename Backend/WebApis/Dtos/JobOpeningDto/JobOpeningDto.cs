using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApis.Data;

namespace WebApis.Dtos.JobOpeningDto
{
    public class jobDocumentDto
    {
        [Required]
        public int DocumentId { get; set; }
        public bool IsMandatory { get; set; } = true;
    }

    public class jobSkillDto
    {
        [Required]
        public string SkillName { get; set; }

        public int minExperience { get; set; } = 0;

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

        [Required]
        public Domain Domain { get; set; }
        public int minDomainExperience { get; set; } = 0;

        public string SalaryRange { get; set; } = "not-specified";

        
        public JobLocation Location { get; set; } = JobLocation.OnSite;

        [Required]
        public Department Department { get; set; }

        
        public JobType JobType { get; set; } = JobType.FullTime;

        
        public EducationLevel Education { get; set; } = EducationLevel.Bachelors;

        
        public JobStatus Status { get; set; } = JobStatus.Open;

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

        [Required]
        public List<jobDocumentDto> Documents { get; set; } = new();

        [Required]
        public List<jobSkillDto>? JobSkills { get; set; } = new();
    }
    
    public enum JobType
    {
        FullTime = 1,
        PartTime = 2,
        Contract = 3,
        Internship = 4,
        Temporary = 5
    }

    public enum EducationLevel
    {
        HighSchool = 1,
        Diploma = 2,
        Bachelors = 3,
        Masters = 4,
        Doctorate = 5,
        Other = 6
    }

    public enum JobStatus
    {
        Open = 1,
        Closed = 2,
        OnHold = 3
    }
    public enum JobLocation
    {
        Remote = 1,
        OnSite = 2,
        Hybrid = 3
    }
}
