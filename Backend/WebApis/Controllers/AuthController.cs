using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Service;

namespace WebApis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwt;

        public AuthController(AppDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("register-candidate")]
        public async Task<IActionResult> RegisterCandidate([FromBody] RegisterRequestDto dto)
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
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var candidate = new Candidate
            {
                UserId = user.Id,
                Education = dto.Education,
                LinkedInProfile = dto.LinkedInProfile,
                GitHubProfile = dto.GitHubProfile,
                ResumePath = dto.ResumePath,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.Candidates.Add(candidate);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Candidate created successfully by Recruiter.",
                user = new { id = user.Id, name = user.FullName, email = user.Email, role = user.RoleName }
            });
        }

        [HttpPost("register-Users")]//register recruiter reviewer and interviewer by admin 
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDto dto)
        {
            //  Validate role
            var allowedRoles = new[] { "Recruiter", "Reviewer", "Interviewer" };
            if (!allowedRoles.Contains(dto.RoleName))
                return BadRequest(new { message = "Invalid role. Admin can only create Recruiter, Reviewer, or Interviewer." });

            //  Check email duplication
            var existingUser = await _db.Users.AnyAsync(u => u.Email == dto.Email.ToLower());
            if (existingUser)
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
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Create role-specific table
            switch (dto.RoleName)
            {
                case "Recruiter":
                    _db.Recruiter.Add(new Recruiter
                    {
                        UserId = user.Id,
                        Department = dto.Department ?? "",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                   
                    break;

                case "Reviewer":
                    _db.Reviewers.Add(new Reviewer
                    {
                        UserId = user.Id,
                        Department = dto.Department ?? "",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    break;

                case "Interviewer":
                    _db.Interviewers.Add(new Interviewer
                    {
                        UserId = user.Id,
                        Department = dto.Department ?? "",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    break;
            }

            await _db.SaveChangesAsync();

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
                    email = user.Email
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
