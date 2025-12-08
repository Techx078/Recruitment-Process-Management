using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApis.Data;


namespace WebApis.Controllers.UserController.InteviewerController
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
        [Authorize(Roles = "Recruiter,Interviewer,Admin,Reviewer")]
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

        [HttpGet("{id}")]
        [Authorize(Roles = "Reviewer,Admin,Recruiter,Interviewer")]
        public async Task<IActionResult> GetInterviewerById(int id)
        {
            var interviewer = await _db.Interviewers
                .Include(i => i.User)
                .Include(i => i.JobInterviewers)
                    .ThenInclude(ji => ji.JobOpening)
                 .Include(i => i.JobInterviewers)
                    .ThenInclude( ji => ji.JobOpening)
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
                    AssignedJobOpenings = i.JobInterviewers
                        .Select(j => new
                        {
                            j.JobOpeningId,
                            j.JobOpening.Title,
                            j.JobOpening.Status,
                            j.JobOpening.Department,
                            j.JobOpening.JobType,
                            j.JobOpening.CreatedAt,
                            CandidateCount = j.JobOpening.JobCandidates.Count,
                            InterviewerCount = j.JobOpening.JobInterviewers.Count,
                            j.JobOpening.minDomainExperience,
                            j.JobOpening.Domain
                            
                        })
                })
                .FirstOrDefaultAsync();
            if (interviewer == null)
            {
                return NotFound();
            }
            return Ok(interviewer);
        }
    }
}
