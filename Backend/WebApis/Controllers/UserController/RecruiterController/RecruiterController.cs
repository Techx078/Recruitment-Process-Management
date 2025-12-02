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
        [HttpGet("{UserId}")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> GetRecruiterById(int UserId)
        {
            var recruiter = await _db.Recruiter.Where(r => r.UserId == UserId).FirstOrDefaultAsync();
            Console.WriteLine("Recruiter ID: " + recruiter?.Id);
            var CreatedJobOpenings = await _db.JobOpening
                                    .AsNoTracking()
                                    .Where(j => j.CreatedById == recruiter.Id )
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

            var recruiterDetail = await _db.Recruiter
                .Where(r => r.Id == recruiter.Id )
                .Include(i => i.User)
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
            if (recruiterDetail == null)
            {
                return NotFound(new { Message = "Recruiter not found." });
            }
            return Ok(recruiterDetail);
        }
    }
}
