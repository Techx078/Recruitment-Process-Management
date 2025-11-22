using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApis.Data; // for enums: JobType, JobStatus, etc.

namespace WebApis.Dtos.JobOpeningDto
{
    public class JobOpeningUpdateDto
    {
      
        public class JobOpeningFieldUpdateDto
        {
            // --- Basic Fields ---

            [Required]
            public string Title { get; set; }

            [Required]
            public string Description { get; set; }

            public string? SalaryRange { get; set; }

            [Required]
            public JobLocation Location { get; set; }

            [Required]
            public Department Department { get; set; }

            [Required]
            public JobType JobType { get; set; }

            [Required]
            public EducationLevel Education { get; set; }

            [Required]
            public JobStatus Status { get; set; }

            public string? Experience { get; set; }   // e.g. "2–5 years"

            // --- JSON List Fields ---
            public List<string>? Responsibilities { get; set; }

            public List<string>? Requirement { get; set; }

            public List<string>? Benefits { get; set; }

            // --- Deadline ---
            [Required]
            public DateTime DeadLine { get; set; }
        }
    }
}

