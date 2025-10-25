public class RegisterRequestDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string RoleName { get; set; }

    // Candidate-specific fields
    public string? Education { get; set; }
    public string? LinkedInProfile { get; set; }
    public string? GitHubProfile { get; set; }
    public string? ResumePath { get; set; }

    // Common for Recruiter/Reviewer/Interviewer
    public string? Department { get; set; }
}
