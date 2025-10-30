using WebApis.Dtos;

public class RegisterRequestDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string RoleName { get; set; }

    // Candidate-specific fields
    public string? Education { get; set; } = null;
    public string? LinkedInProfile { get; set; } = null;
    public string? GitHubProfile { get; set; } = null;
    public string? ResumePath { get; set; } = null;

    // Common for Recruiter/Reviewer/Interviewer
    public string? Department { get; set; } = null;

    public List<SkillsDto>? Skills { get; set; } = null;
}
