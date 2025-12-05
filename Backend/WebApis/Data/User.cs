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
        public Domain Domain { get; set; } 
        public int DomainExperienceYears { get; set; }
        public Candidate candidate { get; set; }
        public Recruiter recruiter { get; set; }
        public Reviewer reviewer { get; set; }
        public Interviewer interviewer { get; set; }
        public ICollection<UserSkill> UserSkills { get; set; }


    }
        public enum Domain
    {
            NotSpecified = 0,
            FullStackDevelopment = 1,
            FrontendDevelopment = 2,
            BackendDevelopment = 3,
            MobileAppDevelopment = 4,
            DataScience = 5,
            ArtificialIntelligence_ML = 6, 
            CloudComputing = 7,            
            DevOps = 8,
            IndustrialIoT = 9,            
            EmbeddedSystems = 10,          
            AutomationEngineering = 11,
            SupplyChainTech = 12,          
            QualityAssurance = 13,
            CyberSecurity = 14
        }
}
