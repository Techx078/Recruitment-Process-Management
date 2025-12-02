using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApis.Data;


using WebApis.Dtos;
using WebApis.Repository;


namespace WebApis.Controllers.UserController.ReviewerController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewerController : Controller
    {
        private readonly AppDbContext _db;
        public ReviewerController(AppDbContext db)
        {
            _db = db;
        }
        [HttpGet("All")]
        [Authorize(Roles = "Reviewer,Admin,Recruiter,Interviewer")]
        public async Task<IActionResult> GetAllReviewer()
        {
            var reviewers = await _db.Reviewers
                            .Include(r => r.User)
                                .ThenInclude(u => u.UserSkills)
                                .ThenInclude(s => s.Skill)
                            .Include(r => r.JobReviewers)
                                .ThenInclude(Jr => Jr.JobOpening)
                            .Select(r => new
                            {
                                r.Id,
                                r.Department,
                                User = new
                                {
                                    r.User.Id,
                                    r.User.FullName,
                                    r.User.Email,
                                    Skills = r.User.UserSkills
                                            .Select(s => new
                                            {
                                                Id = s.Skill.SkillId,
                                                s.Skill.Name
                                            })

                                },
                                Jobs = r.JobReviewers
                                .Select(j => new
                                {
                                    j.JobOpeningId,
                                    j.JobOpening.Title
                                })
                            })
                             .ToListAsync();


            return Ok(reviewers);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Reviewer,Admin,Recruiter,Interviewer")]
        public async Task<IActionResult> GetReviewerById(int id)
        {
            var reviewer = await _db.Reviewers
                .Include(i => i.User)
                .Include(i => i.JobReviewers)
                    .ThenInclude(ji => ji.JobOpening)
                 .Include(i => i.JobReviewers)
                    .ThenInclude(ji => ji.JobOpening)
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
                    AssignedJobOpenings = i.JobReviewers
                        .Select(j => new
                        {
                            j.JobOpeningId,
                            j.JobOpening.Title,
                            j.JobOpening.Status,
                            j.JobOpening.Department,
                            j.JobOpening.JobType,
                            j.JobOpening.CreatedAt,
                            CandidateCount = j.JobOpening.JobCandidates.Count,
                            ReviewerCount = j.JobOpening.JobReviewers.Count,
                            j.JobOpening.Experience,

                        })
                })
                .FirstOrDefaultAsync();
            if (reviewer == null)
            {
                return NotFound(new { Message = "Reviewer not found." });
            }
            return Ok(reviewer);
        }
    }
}
