namespace WebApis.Dtos.ForgotPasswordDtos
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string OTP { get; set; }
        public string NewPassword { get; set; }
    }
    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }
}