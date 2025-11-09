using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApis.Data;


namespace WebApis.Controllers.UserController
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewerController : Controller
    {
        private readonly AppDbContext _db;

        public InterviewerController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllInterviewers()
        {
            var interviewers = await _db.Interviewers
                .Include(i => i.User)
                    .ThenInclude(u => u.UserSkills)
                    .ThenInclude(s => s.Skill)
                .Include(i => i.JobInterviewers)
                    .ThenInclude(ji => ji.JobOpening)
                .Select(i => new
                {
                    i.Id,
                    i.Department,
                    User = new
                    {
                        i.User.Id,
                        i.User.FullName,
                        i.User.Email,
                        Skills = i.User.UserSkills
                            .Select(s => new
                            {
                                Id = s.Skill.SkillId,
                                s.Skill.Name
                            })
                    },
                    Jobs = i.JobInterviewers
                        .Select(j => new
                        {
                            j.JobOpeningId,
                            j.JobOpening.Title
                        })
                })
                .ToListAsync();

            return Ok(interviewers);
        }
    }
}
