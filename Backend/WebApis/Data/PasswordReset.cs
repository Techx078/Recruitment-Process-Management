using System.ComponentModel.DataAnnotations;

namespace WebApis.Data
{
    public class PasswordReset
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]     
        public string Email { get; set; }

        [Required]
        public string OTP { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}
