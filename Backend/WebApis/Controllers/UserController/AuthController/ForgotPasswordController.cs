using MailKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Dtos.ForgotPasswordDtos;
using WebApis.Repository;
using WebApis.Service.EmailService;
using WebApis.Service.ErrorHandlingService;
using WebApis.Service.ErrroHandlingService;
using WebApis.Service.ValidationService;
namespace WebApis.Controllers.UserController.AuthController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForgotPasswordController : Controller
    {
        private readonly IEmailService _emailService;
        ICommonRepository<PasswordReset> _passwordResetRepository;
        ICommonRepository<User> _userRepository;
        ICommonValidator<ResetPasswordDto> _resetPassvalidator;
        IAppEmailService _appEmailService;

        public ForgotPasswordController(
            IEmailService emailService,
            ICommonRepository<PasswordReset> passwordResetRepository,
            ICommonRepository<User> userRepository,
            ICommonValidator<ResetPasswordDto> resetPassvalidator,
            IAppEmailService appEmailService
        )
        {
            _emailService = emailService;
            _passwordResetRepository = passwordResetRepository;
            _userRepository = userRepository;
            _resetPassvalidator = resetPassvalidator;
            _appEmailService = appEmailService;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return Ok(new { Message = "otp sent to given email" });

            var user = await _userRepository.GetByFilterAsync(u => u.Email == dto.Email);

            if (user == null)
                return Ok(new { Message = "otp sent to given email" });

            var otp = Random.Shared.Next(100000, 999999).ToString();

            await _passwordResetRepository.AddAsync(new PasswordReset
            {
                Email = user.Email,
                OTP = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            });

            await _appEmailService.SendPasswordResetOtpAsync(user, otp);

            return Ok(new { Message = "otp sent to given email" });
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody]ResetPasswordDto dto)
        {
            var validationResult = await _resetPassvalidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppException(
                  "Email, OTP and new password are required.",
                  ErrorCodes.ValidationError,
                  StatusCodes.Status400BadRequest
                );
            }

            var entry = await _passwordResetRepository.GetByorderAsync(
                p => p.Email == dto.Email && p.OTP == dto.OTP,
                p => p.Id,
                descending: true
            );

            if (entry == null)
            {
                throw new AppException(
                    "Invalid OTP.",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );
            }

            if (entry.ExpiresAt < DateTime.UtcNow)
            {
                throw new AppException(
                    "OTP expired.",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                );
            }

            var user = await _userRepository.GetByFilterAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                throw new AppException(
                    "User not found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _userRepository.UpdateAsync(user);

            await _passwordResetRepository.DeleteAsync(entry);

            return Ok(new { message = "Password reset successfully." });
        }

    }
}
