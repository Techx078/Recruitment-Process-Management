namespace WebApis.Data
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string RoleName { get; set; } // Could use enum
        public DateTime CreatedAt { get; set; }

        public Candidate candidate { get; set; }
        public ICollection<UserSkill> UserSkills { get; set; }
    }
}
