using System.ComponentModel.DataAnnotations;

namespace WebApis.Dtos
{
    public class SkillsDto
    {
        public string Name { get; set; } = string.Empty;
        public Decimal Experience { get; set; } // in years
    }
}
