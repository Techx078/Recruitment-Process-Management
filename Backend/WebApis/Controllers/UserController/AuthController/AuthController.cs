using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Repository;
using WebApis.Service;

namespace WebApis.Controllers.UserController.AuthController
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwt;
        private readonly CloudinaryService _cloudinaryService;

        //repositories
        private readonly ICommonRepository<User> _userRepository;
        private readonly ICommonRepository<Candidate> _candidateRepository;
        private readonly ICommonRepository<Recruiter> _recruiterRepository;
        private readonly ICommonRepository<Reviewer> _reviewerRepository;
        private readonly ICommonRepository<Interviewer> _interviewerRepository;
        private readonly ICommonRepository<Skill> _skillRepository;

        public AuthController(AppDbContext db, 
            JwtService jwt,
            CloudinaryService cloudinaryService,

            ICommonRepository<User> commonRepository,
            ICommonRepository<Candidate> candidateRepository,
            ICommonRepository<Recruiter> recruiterRepository,
            ICommonRepository<Reviewer> reviewerRepository,
            ICommonRepository<Interviewer> interviewerRepository,
            ICommonRepository<Skill> skillRepository
        )
        {
            _db = db;
            _jwt = jwt;
            _cloudinaryService = cloudinaryService;

            _userRepository = commonRepository;
            _candidateRepository = candidateRepository;
            _recruiterRepository = recruiterRepository;
            _interviewerRepository = interviewerRepository;
            _reviewerRepository = reviewerRepository;
            _skillRepository = skillRepository;
        }


        //Implementation remaining that cadidate should attached to job opening when created by recruiter
        [HttpPost("register-candidate")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> RegisterCandidate([FromBody] RegisterCandidateRequestDto dto)
        {
            if (dto.RoleName != "Candidate")
                return BadRequest(new { message = "Role should be cadidate" });

            var existingUser = await _db.Users.AnyAsync(u => u.Email == dto.Email.ToLower());
            if (existingUser)
                return BadRequest(new { message = "Email already registered." });

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = passwordHash,
                RoleName = "Candidate",
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
            var candidate = new Candidate
            {
                UserId = user.Id,
                LinkedInProfile = dto.LinkedInProfile,
                GitHubProfile = dto.GitHubProfile,
                ResumePath = dto.ResumePath,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _candidateRepository.AddAsync(candidate);
          
            // Add Education Records
            if (dto.Educations != null && dto.Educations.Any())
            {
                foreach (var edu in dto.Educations)
                {
                    var education = new Education
                    {
                        CandidateId = candidate.Id,
                        Degree = edu.Degree,
                        University = edu.University,
                        College = edu.College,
                        PassingYear = edu.PassingYear,
                        Percentage = edu.Percentage
                    };

                    _db.Educations.Add(education);
                }

                await _db.SaveChangesAsync();
            }

            // Add Skills Records
            if (dto.Skills != null && dto.Skills.Any())
            {
                foreach (var skillDto in dto.Skills)
                {
                    var skillName = skillDto.Name.Trim().ToLower();
                    var skill = await _db.Skill.FirstOrDefaultAsync(s => s.Name.ToLower() == skillName);

                    if (skill == null)
                    {
                        skill = new Skill { Name = skillDto.Name };
                        await _skillRepository.AddAsync(skill);
                    }

                    var userSkill = new UserSkill
                    {
                        UserId = user.Id,
                        SkillId = skill.SkillId,
                        YearsOfExperience = skillDto.Experience,
                        ProficiencyLevel = "begineer",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _db.UserSkill.Add(userSkill);
                }
                await _db.SaveChangesAsync();
            }

            return Ok(new
            {
                message = "Candidate created successfully by Recruiter.",
                user = new { id = user.Id, name = user.FullName, email = user.Email, role = user.RoleName }
            });
        }

        [HttpPost("upload-resume")]
        [Authorize(Roles = "Recruiter,Candidate" )]
        public async Task<IActionResult> UploadResume(IFormFile file)
        {
            if (file == null)
                return BadRequest("No file received.");

            try
            {
                var url = await _cloudinaryService.UploadResumeAsync(file);
                return Ok(new { resumeUrl = url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("register-Users")]//register recruiter reviewer and interviewer by admin
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterOtherUserRequestDto dto)
        {
            //  Validate role
            var allowedRoles = new[] { "Recruiter", "Reviewer", "Interviewer" };
            if (!allowedRoles.Contains(dto.RoleName))
            {
                Console.WriteLine("Invalid role attempted: " + dto.RoleName);
                return BadRequest(new { message = "Invalid role", });
            }

            //  Check email duplication
            var existingUser = await _userRepository.GetByFilterAsync(u => u.Email == dto.Email.ToLower());
            if (existingUser != null )
                return BadRequest(new { message = "Email already registered." });

            // Create base user
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = passwordHash,
                RoleName = dto.RoleName,
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
            
            // Create role-specific table
            switch (dto.RoleName)
            {
                case "Recruiter":
                    
                    await _recruiterRepository.AddAsync(new Recruiter
                    {
                        UserId = user.Id,
                        Department = dto.Department.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                   
                    break;

                case "Reviewer":
                   await _reviewerRepository.AddAsync(new Reviewer
                    {
                        UserId = user.Id,
                        Department = dto.Department.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    break;

                case "Interviewer":
                   await _interviewerRepository.AddAsync(new Interviewer
                    {
                        UserId = user.Id,
                        Department = dto.Department.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    break;
            }

            if (dto.Skills != null && dto.Skills.Any())
            {
                foreach (var skillDto in dto.Skills)
                {
                    var skillName = skillDto.Name.Trim().ToLower();
                    var skill = await _db.Skill.FirstOrDefaultAsync(s => s.Name.ToLower() == skillName);

                    if (skill == null)
                    {
                        skill = new Skill { Name = skillDto.Name };
                        await _skillRepository.AddAsync(skill);
                    }

                    var userSkill = new UserSkill
                    {
                        UserId = user.Id,
                        SkillId = skill.SkillId,
                        YearsOfExperience = skillDto.Experience,
                        ProficiencyLevel = "begineer",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _db.UserSkill.Add(userSkill);
                }
                await _db.SaveChangesAsync();
            }

            return Ok(new
            {
                message = $"{dto.RoleName} created successfully by Admin.",
                user = new { id = user.Id, name = user.FullName, email = user.Email, role = user.RoleName }
            });
        }
        
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email.ToLower());
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            bool verified = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
            if (!verified)
                return Unauthorized(new { message = "Invalid email or password." });

            var token = _jwt.GenerateToken(user);

            return Ok(new
            {
                token,
                role = user.RoleName,
                user = new
                {
                    id = user.Id,
                    name = user.FullName,
                    email = user.Email,
                    phone = user.PhoneNumber,
                    role = user.RoleName,
                    createdAt = user.CreatedAt
                }
            });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized(new { message = "Invalid token." });

            int userId = int.Parse(userIdClaim);

            var user = await _db.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    id = u.Id,
                    name = u.FullName,
                    email = u.Email,
                    phone = u.PhoneNumber,
                    role = u.RoleName,
                    createdAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }
    }
}
