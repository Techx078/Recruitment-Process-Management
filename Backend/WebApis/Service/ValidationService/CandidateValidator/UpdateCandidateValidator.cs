using WebApis.Dtos;

namespace WebApis.Service.ValidationService.CandidateValidator
{
    public class UpdateCandidateValidator
       : ICommonValidator<UpdateCandidateDto>
    {
        public Task<ValidationResult> ValidateAsync(UpdateCandidateDto dto)
        {
            var result = new ValidationResult();

            if (dto == null)
            {
                result.Errors.Add("Request body cannot be null.");
                return Task.FromResult(result);
            }

            // FullName
            if (string.IsNullOrWhiteSpace(dto.FullName))
                result.Errors.Add("Full name is required.");
            else if (dto.FullName.Length < 3)
                result.Errors.Add("Full name must be at least 3 characters long.");

            // Email
            if (string.IsNullOrWhiteSpace(dto.Email))
                result.Errors.Add("Email is required.");
            else if (!IsValidEmail(dto.Email))
                result.Errors.Add("Please enter a valid email address.");

            // Phone
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                result.Errors.Add("Phone number is required.");
            else if (dto.PhoneNumber.Length > 10)
                result.Errors.Add("Phone number cannot be longer than 15 digits.");

            // Domain
            if (!Enum.IsDefined(typeof(Domain), dto.Domain))
                result.Errors.Add("Domain selection is required.");

            // Domain Experience
            if (dto.DomainExperienceYears < 0 || dto.DomainExperienceYears > 50)
                result.Errors.Add("Experience must be between 0 and 50 years.");

            // LinkedIn
            if (string.IsNullOrWhiteSpace(dto.LinkedInProfile))
                result.Errors.Add("LinkedIn profile URL is required.");
            else if (!IsValidUrl(dto.LinkedInProfile))
                result.Errors.Add("Please enter a valid LinkedIn profile URL.");

            // GitHub
            if (string.IsNullOrWhiteSpace(dto.GitHubProfile))
                result.Errors.Add("GitHub profile URL is required.");
            else if (!IsValidUrl(dto.GitHubProfile))
                result.Errors.Add("Please enter a valid GitHub profile URL.");

            // Resume
            if (string.IsNullOrWhiteSpace(dto.ResumePath))
                result.Errors.Add("Resume path is required.");

            // Skills
            if (dto.Skills == null || !dto.Skills.Any())
            {
                result.Errors.Add("Skills list cannot be empty.");
            }
            else
            {
                ValidateSkills(dto.Skills, result);
            }

            // Educations
            if (dto.Educations == null || !dto.Educations.Any())
            {
                result.Errors.Add("Education details are required.");
            }
            else
            {
                ValidateEducations(dto.Educations, result);
            }

            return Task.FromResult(result);
        }

        public void ValidateSkills(List<SkillsDto> skills, ValidationResult result)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                var skill = skills[i];

                if (string.IsNullOrWhiteSpace(skill.Name))
                    result.Errors.Add($"Skills[{i}]: Skill name is required.");

                if (skill.Experience < 0 || skill.Experience > 50)
                    result.Errors.Add($"Skills[{i}]: Experience must be between 0 and 50 years.");
            }
        }

        public void ValidateEducations(List<EducationDto> educations, ValidationResult result)
        {
            for (int i = 0; i < educations.Count; i++)
            {
                var edu = educations[i];

                if (string.IsNullOrWhiteSpace(edu.Degree))
                    result.Errors.Add($"Educations[{i}]: Degree is required.");

                if (string.IsNullOrWhiteSpace(edu.University))
                    result.Errors.Add($"Educations[{i}]: University is required.");

                if (string.IsNullOrWhiteSpace(edu.College))
                    result.Errors.Add($"Educations[{i}]: College is required.");

                if (edu.PassingYear < 1950 || edu.PassingYear > 2050)
                    result.Errors.Add($"Educations[{i}]: Passing year must be between 1950 and 2050.");

                if (edu.Percentage < 0 || edu.Percentage > 100)
                    result.Errors.Add($"Educations[{i}]: Percentage must be between 0 and 100.");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
