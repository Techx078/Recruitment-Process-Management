using System.ComponentModel.DataAnnotations;

namespace WebApis.Dtos
{
    public class UpdateCandidateDto
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public Domain Domain { get; set; }

        public decimal DomainExperienceYears { get; set; }

        public string LinkedInProfile { get; set; }

        public string GitHubProfile { get; set; }

        public string ResumePath { get; set; }

        public List<SkillsDto>? Skills { get; set; }

        public List<EducationDto>? Educations { get; set; }
    }
}
