using System.ComponentModel.DataAnnotations;
using WebApis.Dtos;

public class RegisterCandidateDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }

    public RoleName RoleName {get; set;}

    public Domain Domain { get; set; } 
    public decimal DomainExperienceYears { get; set; }

    // Candidate-specific fields
    public List<EducationDto>? Educations { get; set; }
    public string LinkedInProfile { get; set; } = null;
    public string GitHubProfile { get; set; } = null;
    public string ResumePath { get; set; } = null;
    public List<SkillsDto>? Skills { get; set; } = null;
}
public class EducationDto
{
    public string Degree { get; set; }

    public string University { get; set; }

    public string College { get; set; }

    public int PassingYear { get; set; }

    public decimal Percentage { get; set; }
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
public enum RoleName
{
    Admin,
    Recruiter,
    Reviewer,
    Candidate,
    Interviewer
}

