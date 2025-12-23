using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApis.Data; // for enums: JobType, JobStatus, etc.

namespace WebApis.Dtos.JobOpeningDto
{
    public class JobOpeningUpdateDto
    {
            public string Title { get; set; }

            public string Description { get; set; }

            public string? SalaryRange { get; set; }

            public JobLocation Location { get; set; }

            public Department Department { get; set; }

            public JobType JobType { get; set; }

            public EducationLevel Education { get; set; }

            public JobStatus Status { get; set; }

        public Domain Domain { get; set; }
        public int minDomainExperience { get; set; } = 0;  // e.g. "2–5 years"

        public List<string>? Responsibilities { get; set; }

        public List<string>? Requirement { get; set; }

        public List<string>? Benefits { get; set; }

        public DateTime DeadLine { get; set; }
    }
}

