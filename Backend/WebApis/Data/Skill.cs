namespace WebApis.Data
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<UserSkill> UserSkills { get; set; }
    }
}
