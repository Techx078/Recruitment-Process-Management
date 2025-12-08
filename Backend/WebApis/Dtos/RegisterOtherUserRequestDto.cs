namespace WebApis.Dtos
{
    public class RegisterOtherUserRequestDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public RoleName RoleName { get; set; }

        public Domain Domain { get; set; }
        public decimal DomainExperienceYears { get; set; }

        public Department Department { get; set; }

        public List<SkillsDto>? Skills { get; set; }
  
    }
        public enum Department
        {
            SoftwareDevelopment = 1,
            DataScience = 2,
            QualityAssurance = 3,
            DevOps = 4,
            CyberSecurity = 5,
            ITSupport = 6,
            ProductManagement = 7,
            HR = 8,
            Sales = 9,
            Marketing = 10,
            Finance = 11,
            Operations = 12,
        }
}
