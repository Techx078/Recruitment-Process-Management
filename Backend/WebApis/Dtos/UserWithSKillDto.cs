namespace WebApis.Dtos
{
    public class UserWithSKillDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Domain { get; set; }
        public decimal DomainExperienceYears { get; set; }

        public List<SkillsDto> Skills { get; set; } = new List<SkillsDto>();
    }

}
