using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApis.Data;


using WebApis.Dtos;
using WebApis.Repository;


namespace WebApis.Controllers.UserController
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
        public async Task<IActionResult> GetAllReviewer()
        {
            var reviewers = await _db.Reviewers
                            .Include(r => r.User)
                                .ThenInclude(u => u.UserSkills)
                                .ThenInclude( s => s.Skill )
                            .Include(r => r.JobReviewers)
                                .ThenInclude(Jr => Jr.JobOpening)
                            .Select(r => new {
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
    }
}
