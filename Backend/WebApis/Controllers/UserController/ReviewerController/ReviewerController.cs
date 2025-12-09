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
                         .Where(i => i.UserId == id)
                         .Select(i => new ReviewerInterviewerDetailsDto
                         {
                             Id = i.Id,
                             Department = i.Department,

                             User = new UserDto
                             {
                                 Id = i.User.Id,
                                 FullName = i.User.FullName,
                                 Email = i.User.Email,
                                 PhoneNumber = i.User.PhoneNumber,
                                 Domain = i.User.Domain,
                                 DomainExperienceYears = i.User.DomainExperienceYears
                             },
                             AssignedJobOpenings = i.JobReviewers
                                 .Select(j => new AssignedJobOpeningDto
                                 {
                                     JobOpeningId = j.JobOpeningId,
                                     Title = j.JobOpening.Title,
                                     Status = j.JobOpening.Status,
                                     Department = j.JobOpening.Department,
                                     JobType = j.JobOpening.JobType,
                                     CreatedAt = j.JobOpening.CreatedAt,

                                     CandidateCount = j.JobOpening.JobCandidates.Count,
                                     ReviewerCount = j.JobOpening.JobReviewers.Count,

                                     MinDomainExperience = j.JobOpening.minDomainExperience,
                                     Domain = j.JobOpening.Domain
                                 })
                                 .ToList()
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
