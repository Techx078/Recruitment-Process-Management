
using Microsoft.IdentityModel.Tokens;
using WebApis.Dtos.ForgotPasswordDtos;
using WebApis.Service.ErrorHandlingService;
using WebApis.Service.ErrroHandlingService;

namespace WebApis.Service.ValidationService.AuthUserVallidator
{
    public class ResetPasswordValidator : ICommonValidator<ResetPasswordDto>
    {
        public Task<ValidationResult> ValidateAsync(ResetPasswordDto dto)
        {
            var result = new ValidationResult();

            if (dto == null)
            {
                result.Errors.Add(
                    "Request body cannot be null."
                );
                return Task.FromResult(result);
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
                result.Errors.Add(
                    "Email is required."
                );

            if (string.IsNullOrWhiteSpace(dto.OTP))
                result.Errors.Add(
                    "OTP is required."
                );

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
                result.Errors.Add(
                    "New password is required."
                );

            return Task.FromResult(result);
        }
    }
}
