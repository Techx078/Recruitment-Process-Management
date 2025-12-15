using WebApis.Dtos;

namespace WebApis.Service.ValidationService.AuthUserVallidator
{
    public class RegisterOtherUserValidator : ICommonValidator<RegisterOtherUserDto>
    {
        public Task<ValidationResult> ValidateAsync(RegisterOtherUserDto dto)
        {
            var result = new ValidationResult();

            // Full Name
            if (string.IsNullOrWhiteSpace(dto.FullName))
                result.Errors.Add("Full name is required.");

            // Email
            if (string.IsNullOrWhiteSpace(dto.Email))
                result.Errors.Add("Email is required.");

            // Phone
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                result.Errors.Add("Phone number is required.");

            // Password
            if (string.IsNullOrWhiteSpace(dto.Password))
                result.Errors.Add("Password is required.");

            // Role
            if (!Enum.IsDefined(typeof(RoleName), dto.RoleName))
                result.Errors.Add("Invalid role name.");

            // Domain
            if (!Enum.IsDefined(typeof(Domain), dto.Domain))
                result.Errors.Add("Invalid domain.");

            // Domain Experience
            if (dto.DomainExperienceYears < 0 || dto.DomainExperienceYears > 50)
                result.Errors.Add("Domain experience must be between 0 and 50 years.");

            // Department
            if (!Enum.IsDefined(typeof(Department), dto.Department))
                result.Errors.Add("Invalid department.");

            // Skills (simple list validation)
            if (dto.Skills != null && !dto.Skills.Any())
                result.Errors.Add("Skills list cannot be empty.");

            // Skills item-level validation
            if (dto.Skills?.Any() == true)
            {
                foreach (var skill in dto.Skills)
                {
                    if (string.IsNullOrWhiteSpace(skill.Name))
                        result.Errors.Add("Skill name is required.");

                    if (skill.Experience < 0)
                        result.Errors.Add($"Experience for skill '{skill.Name}' cannot be negative.");

                    if (skill.Experience > 50)
                        result.Errors.Add($"Experience for skill '{skill.Name}' is not valid.");
                }
            }

            return Task.FromResult(result);
        }
    }
}
