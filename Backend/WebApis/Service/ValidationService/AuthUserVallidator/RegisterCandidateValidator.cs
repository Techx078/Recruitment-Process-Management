
using WebApis.Data;

namespace WebApis.Service.ValidationService.AuthUserVallidator
{
    public class RegisterCandidateValidator : ICommonValidator<RegisterCandidateDto>
    {
        public Task<ValidationResult> ValidateAsync(RegisterCandidateDto dto)
        {
            var result = new ValidationResult();

            if (dto == null)
            {
                result.Errors.Add("Request body cannot be null.");
                return Task.FromResult(result);
            }

            // Full Name
            if (string.IsNullOrWhiteSpace(dto.FullName))
                result.Errors.Add("Full name is required.");
            else if (dto.FullName.Length < 3)
                result.Errors.Add("Full name must be at least 3 characters.");

            // Email
            if (string.IsNullOrWhiteSpace(dto.Email))
                result.Errors.Add("Email is required.");
            else if (!IsValidEmail(dto.Email))
                result.Errors.Add("Email format is invalid.");

            // Phone
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                result.Errors.Add("Phone number is required.");

            // Password
            if (string.IsNullOrWhiteSpace(dto.Password))
                result.Errors.Add("Password is required.");
            else if (dto.Password.Length < 6)
                result.Errors.Add("Password must be at least 6 characters.");

            // Role
            if (!Enum.IsDefined(typeof(RoleName), dto.RoleName))
                result.Errors.Add("Invalid role name.");

            // Domain
            if (!Enum.IsDefined(typeof(Domain), dto.Domain))
                result.Errors.Add("Invalid domain.");

            // Domain Experience
            if (dto.DomainExperienceYears < 0)
                result.Errors.Add("Domain experience years cannot be negative.");

            // LinkedIn
            if (!string.IsNullOrWhiteSpace(dto.LinkedInProfile) &&
                !dto.LinkedInProfile.StartsWith("https://"))
            {
                result.Errors.Add("LinkedIn profile must be a valid URL.");
            }

            // GitHub
            if (!string.IsNullOrWhiteSpace(dto.GitHubProfile) &&
                !dto.GitHubProfile.StartsWith("https://"))
            {
                result.Errors.Add("GitHub profile must be a valid URL.");
            }

            // Educations
            if (dto.Educations == null || !dto.Educations.Any())
                result.Errors.Add("Education list cannot be empty.");

            foreach (var edu in dto.Educations)
            {
                if (string.IsNullOrWhiteSpace(edu.Degree))
                    result.Errors.Add("Degree is required.");

                if (string.IsNullOrWhiteSpace(edu.University))
                    result.Errors.Add("University is required.");

                if (edu.PassingYear < 1950 || edu.PassingYear > DateTime.UtcNow.Year)
                    result.Errors.Add($"Passing year '{edu.PassingYear}' is invalid.");

                if (edu.Percentage < 0 || edu.Percentage > 100)
                    result.Errors.Add("Percentage must be between 0 and 100.");
            }

            // Skills
            if (dto.Skills == null || !dto.Skills.Any())
                result.Errors.Add("Skills list cannot be empty.");

            foreach (var skill in dto.Skills)
            {
                if (string.IsNullOrWhiteSpace(skill.Name))
                    result.Errors.Add("Skill name is required.");

                if (skill.Experience < 0)
                    result.Errors.Add($"Experience for skill '{skill.Name}' cannot be negative.");

                if (skill.Experience > 50)
                    result.Errors.Add($"Experience for skill '{skill.Name}' is not valid.");
            }
            return Task.FromResult(result);
        }
        public bool IsValidEmail(string email)
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
    }
}
