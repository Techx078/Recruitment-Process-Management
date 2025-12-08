namespace WebApis.Data
{
    public class JobSkill
    {
        public int Id { get; set; }
        public int JobOpeningId { get; set; }
        public int SkillId { get; set; }

        // Flag for whether it’s required or preferred
        public bool IsRequired { get; set; } = true;

        public int minExperience { get; set; }

        // Navigation properties
        public JobOpening JobOpening { get; set; }
        public Skill Skill { get; set; }
    }
}
