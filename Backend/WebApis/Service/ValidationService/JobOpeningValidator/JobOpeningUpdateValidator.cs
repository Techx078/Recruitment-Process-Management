using WebApis.Dtos;
using WebApis.Dtos.JobOpeningDto;

namespace WebApis.Service.ValidationService.JobOpeningValidator
{
    public class JobOpeningUpdateValidator : ICommonValidator<JobOpeningUpdateDto>
    {
        public async Task<ValidationResult> ValidateAsync(JobOpeningUpdateDto dto)
        {
            var result = new ValidationResult();

            if (dto == null)
            {
                result.Errors.Add("Request body cannot be null.");
                return result;
            }

            if (string.IsNullOrWhiteSpace(dto.Title))
                result.Errors.Add("Title is required.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                result.Errors.Add("Description is required.");

            if (dto.minDomainExperience < 0)
                result.Errors.Add("Minimum domain experience must be positive.");

            if (dto.DeadLine <= DateTime.UtcNow)
                result.Errors.Add("Deadline must be in the future.");

            if (!Enum.IsDefined(typeof(JobLocation), dto.Location))
                result.Errors.Add("Invalid Job Location.");

            if (!Enum.IsDefined(typeof(Department), dto.Department))
                result.Errors.Add("Invalid Department.");

            if (!Enum.IsDefined(typeof(JobType), dto.JobType))
                result.Errors.Add("Invalid Job Type.");

            if (!Enum.IsDefined(typeof(EducationLevel), dto.Education))
                result.Errors.Add("Invalid Education Level.");

            if (!Enum.IsDefined(typeof(JobStatus), dto.Status))
                result.Errors.Add("Invalid Job Status.");

            if (!Enum.IsDefined(typeof(Domain), dto.Domain))
                result.Errors.Add("Invalid Domain.");

            // --- List Validations (JSON fields) ---
            if (dto.Responsibilities != null && !dto.Responsibilities.Any())
                result.Errors.Add("Responsibilities cannot be empty if provided.");

            if (dto.Requirement != null && !dto.Requirement.Any())
                result.Errors.Add("Requirement cannot be empty if provided.");

            if (dto.Benefits != null && !dto.Benefits.Any())
                result.Errors.Add("Benefits cannot be empty if provided.");

            return result;
        }
    }
}
