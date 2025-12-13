using System.ComponentModel.DataAnnotations;

namespace WebApis.Dtos
{
    public class SkillsDto
    {
        [Required(ErrorMessage = "SkillName is required.")]
        public string Name { get; set; } = string.Empty;
        [Range(0,50, ErrorMessage = "Experience is required.")]
        public Decimal Experience { get; set; } // in years
    }
}
