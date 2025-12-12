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
        [HttpPost("Candidate-bulk-register")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> BulkRegisterCandidates(
        [FromBody] List<RegisterCandidateRequestDto> candidatesDto)
        {
            if (candidatesDto == null || !candidatesDto.Any())
                return BadRequest(new { message = "Candidate list is empty." });

            var resultSkipped = new List<object>();
            // Store candidate references for getting IDs after SaveChanges
            var createdCandidates = new List<(Candidate candidate, User user)>();


            //get all mails from body
            var incomingEmails = candidatesDto
                .Select(x => x.Email.ToLower())
                .ToList();

            //find exists mail to skip
            var alreadyExists = await _db.Users
                .Where(u => incomingEmails.Contains(u.Email))
                .Select(u => u.Email)
                .ToHashSetAsync();

            //get all skills from database
            var existingSkills = await _skillRepository.GetAllAsync();

            //main loop
            foreach (var dto in candidatesDto)
            {
                var email = dto.Email.ToLower();

                if (alreadyExists.Contains(email))
                {
                    resultSkipped.Add(new { email, reason = "Email already exists" });
                    continue;
                }
                if( dto.DomainExperienceYears < 0)
                {
                    resultSkipped.Add(new { email, reason = "Domain experinece can't be negative." });
                    continue;
                }
                // create a user
                var user = new User
                {
                    FullName = dto.FullName,
                    Email = email,
                    PhoneNumber = dto.PhoneNumber,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    RoleName = "Candidate",
                    Domain = dto.Domain.ToString(),
                    DomainExperienceYears = dto.DomainExperienceYears,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Users.Add(user);


                // create cadidate
                var candidate = new Candidate
                {
                    User = user,    
                    LinkedInProfile = dto.LinkedInProfile,
                    GitHubProfile = dto.GitHubProfile,
                    ResumePath = dto.ResumePath,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.Candidates.Add(candidate);


                //create education
                if (dto.Educations?.Any() == true)
                {
                    var educationEntities = dto.Educations.Select(e => new Education
                    {
                        Candidate = candidate,   
                        Degree = e.Degree,
                        University = e.University,
                        College = e.College,
                        PassingYear = e.PassingYear,
                        Percentage = e.Percentage
                    });

                    _db.Educations.AddRange(educationEntities);
                }


                //add skill for candidate
                if (dto.Skills?.Any() == true)
                {
                    foreach (var skillDto in dto.Skills)
                    {
                        var normalized = skillDto.Name.Trim().ToLower();

                        var skill = existingSkills
                            .FirstOrDefault(x => x.Name.ToLower() == normalized);

                        // Create skill if missing
                        if (skill == null)
                        {
                            skill = new Skill { Name = skillDto.Name };

                            _db.Skill.Add(skill);
                            existingSkills.Add(skill);
                        }

                        _db.UserSkill.Add(new UserSkill
                        {
                            User = user,          
                            Skill = skill,
                            YearsOfExperience = skillDto.Experience,
                            ProficiencyLevel = "beginner",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }

                createdCandidates.Add((candidate, user));

            }

            //commin everthing a once
            await _db.SaveChangesAsync();

            var resultCreated = createdCandidates.Select(x => new
            {
                CandidateId = x.Item1.Id,
                ResumePath = x.Item1.ResumePath,
                name = x.Item2.FullName,
                email = x.Item2.Email
            }).ToList();

            return Ok(new
            {
                message = "Bulk candidate registration completed.",
                successCount = resultCreated.Count,
                skippedCount = resultSkipped.Count,
                created = resultCreated,
                skipped = resultSkipped
            });
        }


        //Implementation remaining that cadidate should attached to job opening when created by recruiter
        [HttpPost("register-candidate")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> RegisterCandidate([FromBody] RegisterCandidateRequestDto dto)
        {
            var existingUser = await _db.Users.AnyAsync(u => u.Email == dto.Email.ToLower());
            if (existingUser)
                return BadRequest(new { message = "Email already registered." });

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            if (dto.DomainExperienceYears < 0)
                return BadRequest("Experience must be positive");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = passwordHash,
                Domain = dto.Domain.ToString(),
                DomainExperienceYears = dto.DomainExperienceYears,
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
                id= candidate.Id,
                resumePath=candidate.ResumePath,
                message = "Candidate created successfully by Recruiter.",
                user = new {  id = user.Id, name = user.FullName, email = user.Email, role = user.RoleName , user.Domain,user.DomainExperienceYears }
            });
        }

        [HttpPost("upload-resume")]
        [Authorize(Roles = "Recruiter")]
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
            if (!allowedRoles.Contains(dto.RoleName.ToString()))
            {
                Console.WriteLine("Invalid role attempted: " + dto.RoleName);
                return BadRequest(new { message = "Invalid role", });
            }

            //  Check email duplication
            var existingUser = await _userRepository.GetByFilterAsync(u => u.Email == dto.Email.ToLower());
            if (existingUser != null )
                return BadRequest(new { message = "Email already registered." });

            if(dto.DomainExperienceYears < 0)
                return BadRequest(new { message = "domain experience should be positie " });
            
            // Create base user
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = passwordHash,
                Domain = dto.Domain.ToString(),
                DomainExperienceYears = dto.DomainExperienceYears,
                RoleName = dto.RoleName.ToString(),
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
            
            // Create role-specific table
            switch (dto.RoleName.ToString())
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
                user = new { id = user.Id, name = user.FullName, email = user.Email, role = user.RoleName, user.Domain,user.DomainExperienceYears }
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
