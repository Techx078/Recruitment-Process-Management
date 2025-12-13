using System.ComponentModel.DataAnnotations;
using WebApis.Dtos;

public class RegisterCandidateRequestDto
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
    [Required(ErrorMessage = "Degree is required.")]
    public string Degree { get; set; }

    [Required(ErrorMessage = "University is required.")]
    public string University { get; set; }

    [Required(ErrorMessage = "College is required.")]
    public string College { get; set; }

    [Required(ErrorMessage = "passingYear is required.")]
    [Range(1950,2050,ErrorMessage = "passingyear between 1950 and 2050 is required.")]
    public int PassingYear { get; set; }

    [Required(ErrorMessage = "Percentage is required.")]
    [Range(0,100, ErrorMessage = "percentage shoul be valid.")]
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

