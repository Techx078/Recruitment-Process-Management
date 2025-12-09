using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApis.Data;
using WebApis.Dtos;


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
                        DomainExperienceYears = i.User.DomainExperienceYears,
                    },

                    AssignedJobOpenings = i.JobInterviewers
                        .Select(j => new AssignedJobOpeningDto
                        {
                            JobOpeningId = j.JobOpeningId,
                            Title = j.JobOpening.Title,
                            Status = j.JobOpening.Status,
                            Department = j.JobOpening.Department,
                            JobType = j.JobOpening.JobType,
                            CreatedAt = j.JobOpening.CreatedAt,
                            CandidateCount = j.JobOpening.JobCandidates.Count,
                            InterviewerCount = j.JobOpening.JobInterviewers.Count,
                            MinDomainExperience = j.JobOpening.minDomainExperience,
                            Domain = j.JobOpening.Domain
                        })
                      .ToList()
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
