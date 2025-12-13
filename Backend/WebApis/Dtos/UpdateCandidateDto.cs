using System.ComponentModel.DataAnnotations;

namespace WebApis.Dtos
{
    public class UpdateCandidateDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [MinLength(3, ErrorMessage = "Full name must be at least 3 characters long.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [MaxLength(15, ErrorMessage = "Phone number cannot be longer than 15 digits.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Domain selection is required.")]
        public Domain Domain { get; set; }

        [Required(ErrorMessage = "Domain experience is required.")]
        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years.")]
        public decimal DomainExperienceYears { get; set; }

        [Required(ErrorMessage = "LinkedIn profile URL is required.")]
        [Url(ErrorMessage = "Please enter a valid LinkedIn profile URL.")]
        public string LinkedInProfile { get; set; }

        [Required(ErrorMessage = "GitHub profile URL is required.")]
        [Url(ErrorMessage = "Please enter a valid GitHub profile URL.")]
        public string GitHubProfile { get; set; }

        [Required(ErrorMessage = "Resume path is required.")]
        public string ResumePath { get; set; }

        [Required(ErrorMessage = "Skills list cannot be empty.")]
        public List<SkillsDto>? Skills { get; set; }

        [Required(ErrorMessage = "Education details are required.")]
        public List<EducationDto>? Educations { get; set; }
    }
}
