namespace WebApis.Data
{
    public class UserSkill
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SkillId { get; set; }
        public string ProficiencyLevel { get; set; }
        public Decimal? YearsOfExperience { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Skill Skill { get; set; }

    }
}
