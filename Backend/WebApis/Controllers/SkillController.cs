using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using WebApis.Data;
using WebApis.Repository;

namespace WebApis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : ControllerBase
    {
        private readonly ICommonRepository<Skill> _repository;
        public SkillController( ICommonRepository<Skill> repository)
        {
            _repository = repository;
        }
        [HttpGet("All")]
        public async Task<IActionResult> GetSkills()
        {
            var Skills = await _repository.GetAllAsync();
            return Ok(Skills);
        }
    }
}
