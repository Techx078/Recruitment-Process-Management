using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Dtos.ForgotPasswordDtos;
using WebApis.Repository;
using WebApis.Service.EmailService;
namespace WebApis.Controllers.UserController.AuthController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForgotPasswordController : Controller
    {
        private readonly IEmailService _emailService;
        ICommonRepository<PasswordReset> _passwordResetRepository;
        ICommonRepository<User> _userRepository;

        public ForgotPasswordController(
            IEmailService emailService,
            ICommonRepository<PasswordReset> passwordResetRepository,
            ICommonRepository<User> userRepository
        )
        {
            _emailService = emailService;
            _passwordResetRepository = passwordResetRepository;
            _userRepository = userRepository;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] String Email)
        {
           
            var user = await _userRepository.GetByFilterAsync(u => u.Email == Email);
            if (user == null)
                return BadRequest(new { message = "Email not found" });

            var otp = new Random().Next(100000, 999999).ToString(); // 6 digit OTP

            // Save OTP , expiry (5 mins)
            var resetEntry = new PasswordReset
            {
                Email = user.Email,
                OTP = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            await _passwordResetRepository.AddAsync(resetEntry);
           
            // Send email 
            await _emailService.SendEmailAsync(
                user.Email,
                "Your Password Reset OTP",
                $"Your OTP is: {otp}. It will expire in 5 minutes."
            );

            return Ok(new { message = "OTP sent to email" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            //. Check OTP entry
            var entry = await _passwordResetRepository.GetByorderAsync(
                        p => p.Email == dto.Email && p.OTP == dto.OTP,
                        p => p.Id,
                        descending: true
                    );

            if (entry == null)
                return BadRequest(new { message = "Invalid OTP" });

            if (entry.ExpiresAt < DateTime.UtcNow)
                return BadRequest(new { message = "OTP expired" });

            // Get user
            var user = await _userRepository.GetByFilterAsync(u => u.Email == dto.Email);
            if (user == null)
                return BadRequest(new { message = "User not found" });

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _userRepository.UpdateAsync(user);
            // Remove OTP entry so it cannot be reused
            await _passwordResetRepository.DeleteAsync(entry);
          
            return Ok(new { message = "Password reset successful" });
        }
    }
}
