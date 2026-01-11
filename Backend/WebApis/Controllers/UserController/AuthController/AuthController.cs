using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Dtos.MailDtos;
using WebApis.Repository;
using WebApis.Repository.UserRepository;
using WebApis.Service;
using WebApis.Service.EmailService;
using WebApis.Service.ErrorHandlingService;
using WebApis.Service.ErrroHandlingService;
using WebApis.Service.ValidationService;
using WebApis.Service.ValidationService.AuthUserVallidator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace WebApis.Controllers.UserController.AuthController
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwt;
        private readonly CloudinaryService _cloudinaryService;

        //repositories
        private readonly ICommonRepository<User> _userRepository;
        private readonly ICommonRepository<Candidate> _candidateRepository;
        private readonly ICommonRepository<Recruiter> _recruiterRepository;
        private readonly ICommonRepository<Reviewer> _reviewerRepository;
        private readonly ICommonRepository<Interviewer> _interviewerRepository;
        private readonly ICommonValidator<RegisterCandidateDto> _registerCandidateValidator;
        private readonly IUserRepository _UserRepository;
        private readonly ICommonValidator<RegisterOtherUserDto> _registerOtherUserValidator;
        private readonly IAppEmailService _appEmailService;
        public AuthController(
            JwtService jwt,
            CloudinaryService cloudinaryService,
            ICommonValidator<RegisterCandidateDto> registerCandidateValidator,
            ICommonRepository<User> commonRepository,
            ICommonRepository<Candidate> candidateRepository,
            ICommonRepository<Recruiter> recruiterRepository,
            ICommonRepository<Reviewer> reviewerRepository,
            ICommonRepository<Interviewer> interviewerRepository,
            IUserRepository UserRepository,
            ICommonValidator<RegisterOtherUserDto> registerOtherUserValidator,
            IAppEmailService appEmailService
        )
        {
            _jwt = jwt;
            _cloudinaryService = cloudinaryService;
            _registerCandidateValidator = registerCandidateValidator;
            _userRepository = commonRepository;
            _candidateRepository = candidateRepository;
            _recruiterRepository = recruiterRepository;
            _interviewerRepository = interviewerRepository;
            _reviewerRepository = reviewerRepository;
            _UserRepository = UserRepository;
            _registerOtherUserValidator = registerOtherUserValidator;
            _appEmailService = appEmailService;
        }
        [HttpPost("Candidate-bulk-register")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> BulkRegisterCandidates(
        [FromBody] List<RegisterCandidateDto> candidatesDto)
        {
            if (candidatesDto == null || !candidatesDto.Any())
                return BadRequest(new { message = "Candidate list is empty." });

            var failureEmails = new List<BulkCandidateFailureDto>();
            // Store candidate references for getting IDs after SaveChanges
            var createdCandidates = new List<(Candidate candidate, User user)>();

            //main loop
            foreach (var dto in candidatesDto)
            {
                var validationResult = await _registerCandidateValidator.ValidateAsync(dto);

                if (!validationResult.IsValid)
                {
                    failureEmails.Add(new BulkCandidateFailureDto
                    {
                        Email = dto.Email,
                        Reason = "Validation error"
                    });
                    continue;
                }
                if (await _userRepository.ExistsAsync( u => u.Email == dto.Email.ToLower()))
                {
                    failureEmails.Add(new BulkCandidateFailureDto
                    {
                        Email = dto.Email,
                        Reason = "Email already exists"
                    });
                    continue;
                }

                // create a user
                var user = new User
                {
                    FullName = dto.FullName,
                    Email = dto.Email.ToLower(),
                    PhoneNumber = dto.PhoneNumber,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    RoleName = "Candidate",
                    Domain = dto.Domain.ToString(),
                    DomainExperienceYears = dto.DomainExperienceYears,
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
               
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

                await _candidateRepository.AddAsync(candidate);

                //create education
                await _UserRepository.AddEducationsAsync(candidate, dto.Educations);
                
                //add skill for candidate
                await _UserRepository.AddSkillsAsync(user, dto.Skills);
              
                createdCandidates.Add((candidate, user));

            }

            foreach (var item in createdCandidates)
            {
                await _appEmailService.SendCandidateRegistrationEmailAsync(
                    item.user,
                    candidatesDto.First(x => x.Email == item.user.Email).Password
                );
            }

            if (failureEmails.Any())
            {
                await _appEmailService.SendBulkCandidateFailureReportAsync(
                    User.FindFirstValue(ClaimTypes.Email)!,
                    failureEmails
                );
            }


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
                skippedCount = failureEmails.Count,
                created = resultCreated,
                skipped = failureEmails
            });
        }

        //Implementation remaining that cadidate should attached to job opening when created by recruiter
        [HttpPost("register-candidate")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> RegisterCandidate([FromBody] RegisterCandidateDto dto)
        {
            var validationResult = await _registerCandidateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new AppException(
                    "Please fill all required fields correctly.",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest,
                    validationResult.Errors
                );
            }
            if (await _userRepository.ExistsAsync( u => u.Email == dto.Email)) {
                throw new AppException(
                     "Email already registered !",
                     ErrorCodes.Duplicate,
                     StatusCodes.Status409Conflict
                );
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
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

            await _UserRepository.AddEducationsAsync(candidate, dto.Educations);

            await _UserRepository.AddSkillsAsync(user,dto.Skills);

            await _appEmailService.SendCandidateRegistrationEmailAsync(
                user,
                dto.Password
            );

            return Ok(new
            {
                id = candidate.Id,
                resumePath = candidate.ResumePath,
                message = "Candidate created successfully by Recruiter.",
            });
        }

        [HttpPost("upload-resume")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UploadResume(IFormFile file)
        {
            if (file == null)
              throw new AppException(
                "File is Required !",
                ErrorCodes.ValidationError,
                StatusCodes.Status400BadRequest
              );      
                
               var url = await _cloudinaryService.UploadResumeAsync(file);
                if (url == null)
                {
                    throw new AppException(
                    "faied to upload resume !",
                    ErrorCodes.ServerError,
                    StatusCodes.Status500InternalServerError
                  );
                }

                return Ok(new { resumeUrl = url });
        }

        [HttpPost("register-Users")]//register recruiter reviewer and interviewer by admin
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterOtherUserDto dto)
        {
            //  Validate role
            var allowedRoles = new[] { "Recruiter", "Reviewer", "Interviewer" };
            if (!allowedRoles.Contains(dto.RoleName.ToString()))
            {
                throw new AppException(
                 "Role should be Recruiter, Reviewer or Interviewer ",
                 ErrorCodes.ValidationError,
                 StatusCodes.Status400BadRequest
                );
            }

            var validationResult = await _registerOtherUserValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new AppException(
                    "Please fill all required fields correctly.",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest,
                    validationResult.Errors
                );
            }
            //  Check email duplication
            if (await _userRepository.ExistsAsync( u => u.Email == dto.Email.ToLower()))
                throw new AppException(
                    "Email already registered !",
                    ErrorCodes.Duplicate,
                    StatusCodes.Status409Conflict
               );

            // Create base user
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
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

            await _UserRepository.AddSkillsAsync(user, dto.Skills);
            await _appEmailService.SendInternalUserRegistrationEmailAsync(
                user,
                dto.Password
            );

            return Ok(new
            {
                message = $"{dto.RoleName} created successfully.",
            });
        }
        
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _userRepository.GetByFilterAsync(u => u.Email == req.Email.ToLower());
            if (user == null)
                throw new AppException(
                 "Invalid email or password.",
                 ErrorCodes.Unauthorized,
                 StatusCodes.Status401Unauthorized
             );

            bool verified = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
            if (!verified)
                throw new AppException(
                 "Invalid email or password.",
                 ErrorCodes.Unauthorized,
                 StatusCodes.Status401Unauthorized
                );

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
                throw new AppException(
                "Invalid tokem",
                ErrorCodes.NotFound,
                StatusCodes.Status404NotFound
                );

            int userId = int.Parse(userIdClaim);

            var user = await _userRepository.GetByFilterAsync(
                         u => u.Id == userId,
                         u => new
                         {
                             id = u.Id,
                             name = u.FullName,
                             email = u.Email,
                             phone = u.PhoneNumber,
                             role = u.RoleName,
                             createdAt = u.CreatedAt
                         }
                     );

            if (user == null)
                throw new AppException(
                    "Record not found.",
                    ErrorCodes.NotFound,
                    StatusCodes.Status404NotFound
                );
           
            return Ok(user);
        }
    }
}
