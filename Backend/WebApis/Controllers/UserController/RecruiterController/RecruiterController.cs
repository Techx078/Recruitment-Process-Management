using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApis.Data;


namespace WebApis.Controllers.UserController.RecruiterController
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecruiterController : Controller
    {
        private readonly AppDbContext _db;
        public RecruiterController(AppDbContext db)
        {
            _db = db;
        }
        [HttpGet("All")]
        public async Task<IActionResult> GetAllRecruiters()
        {
            var Recruiters = await _db.Recruiter
                .Include(i => i.User)
                    .ThenInclude(u => u.UserSkills)
                    .ThenInclude(s => s.Skill)
                
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
                   
                })
                .ToListAsync();

            return Ok(Recruiters);
        }
        [HttpGet("id")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetRecruiterById(int id)
        {
            var CreatedJobOpenings = await _db.JobOpening
                                    .AsNoTracking()
                                    .Where(j => j.CreatedById == id )
                                    .Select(j => new
                                    {
                                        j.Id,
                                        j.Title,
                                        j.Status,
                                        j.Department,
                                        j.JobType,
                                        j.CreatedAt,
                                        CandidateCount = j.JobCandidates.Count,
                                        j.Experience,
                                    })
                                    .ToListAsync();

            var recruiter = await _db.Recruiter
                .Include(i => i.User)
                .Where(i => i.UserId == id)
                .Select(i => new
                {
                    i.Id,
                    i.Department,
                    User = new
                    {
                        i.User.Id,
                        i.User.FullName,
                        i.User.Email,
                        i.User.PhoneNumber,
                    },
                    CreatedJobOpenings
                })
                .FirstOrDefaultAsync();
            if (recruiter == null)
            {
                return NotFound(new { Message = "Recruiter not found." });
            }
            return Ok(recruiter);
        }
    }
}
