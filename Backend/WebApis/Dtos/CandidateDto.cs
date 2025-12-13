namespace WebApis.Dtos
{
    public class CandidateDto
    {
        public int Id { get; set; }
        public string LinkedInProfile { get; set; }
        public string GitHubProfile { get; set; }
        public string? ResumePath { get; set; }
        public int UserId { get; set; }
        public UserWithSKillDto User { get; set; }

        public List<EducationDto> Educations { get; set; } = new();
    }
}
