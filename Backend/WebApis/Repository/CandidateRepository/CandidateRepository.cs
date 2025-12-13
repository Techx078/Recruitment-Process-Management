using Microsoft.EntityFrameworkCore;
using WebApis.Data;
using WebApis.Dtos;

namespace WebApis.Repository.CandidateRepository
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly AppDbContext _db;
        public CandidateRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CandidateDto> GetCandidateDetailsByUserId(int UserId)
        {
            var candidate = await _db.Candidates
                .Where(c => c.UserId == UserId)
                .Select(c => new CandidateDto
                {
                    User = new UserWithSKillDto
                    {
                        Id = c.UserId,
                        FullName = c.User.FullName,
                        Email = c.User.Email,
                        PhoneNumber = c.User.PhoneNumber,
                        Domain = c.User.Domain,
                        RoleName = c.User.RoleName,
                        DomainExperienceYears = c.User.DomainExperienceYears,
                        Skills = c.User.UserSkills.Select(us => new SkillsDto
                        {
                            Name = us.Skill.Name,
                            Experience = Convert.ToDecimal(us.YearsOfExperience)
                        }).ToList(),
                    },
                    Educations = c.Educations.Select(e => new EducationDto
                    {
                        Degree = e.Degree,
                        University = e.University,
                        College = e.College,
                        PassingYear = e.PassingYear,
                        Percentage = e.Percentage,
                    }).ToList(),
                    LinkedInProfile = c.LinkedInProfile,
                    GitHubProfile = c.GitHubProfile,
                    ResumePath = c.ResumePath,
                    Id = c.Id,
                    UserId = c.UserId,
                })
                .FirstOrDefaultAsync();
            if (candidate == null)
            {
               throw new KeyNotFoundException("Candidate not found !");
            }
            return candidate;
        }

        public async Task<Candidate?> GetCandidateWithUserAsync(int userId)
        {
            return await _db.Candidates
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<List<CandidateJobDto>> GetCnadidateJobOpeningDetailsByUserId(int UserId)
        {
            var jobOpenings = await _db.Candidates
               .Where(c => c.UserId == UserId)
               .Select(c => c.JobCandidates
                   .Select(jc => new CandidateJobDto
                   {
                       Id = jc.Id,
                       candidateId = jc.CandidateId,
                       JobOpeningId = jc.JobOpeningId,
                       JobTitle = jc.JobOpening.Title,
                       JobStatus = jc.JobOpening.Status,
                       CvPath = jc.CvPath,
                       Status = jc.Status,
                       RoundNumber = jc.RoundNumber,
                   }).ToList()
               )
               .FirstOrDefaultAsync();
            if (jobOpenings == null)
            {
                throw new KeyNotFoundException("not found !");
            }
            return jobOpenings;
        }

        public async Task UpdateCandidateAsync(Candidate candidate, UpdateCandidateDto dto)
        {
            candidate.User.FullName = dto.FullName;
            candidate.User.Email = dto.Email.ToLower();
            candidate.User.PhoneNumber = dto.PhoneNumber;
            candidate.User.Domain = dto.Domain.ToString();
            candidate.User.DomainExperienceYears = dto.DomainExperienceYears;
            candidate.LinkedInProfile = dto.LinkedInProfile;
            candidate.GitHubProfile = dto.GitHubProfile;
            candidate.ResumePath = dto.ResumePath;
            candidate.UpdatedAt = DateTime.UtcNow;
            var oldEducations = _db.Educations.Where(e => e.CandidateId == candidate.Id);
            _db.Educations.RemoveRange(oldEducations);

            if (dto.Educations != null)
            {
                foreach (var edu in dto.Educations)
                {
                    _db.Educations.Add(new Education
                    {
                        CandidateId = candidate.Id,
                        Degree = edu.Degree,
                        University = edu.University,
                        College = edu.College,
                        PassingYear = edu.PassingYear,
                        Percentage = edu.Percentage
                    });
                }
            }
            var oldSkills = _db.UserSkill.Where(us => us.UserId == candidate.UserId);
            _db.UserSkill.RemoveRange(oldSkills);

            if (dto.Skills != null)
            {
                foreach (var skillDto in dto.Skills)
                {
                    var skillName = skillDto.Name.Trim().ToLower();
                    var skill = await _db.Skill.FirstOrDefaultAsync(s => s.Name.ToLower() == skillName);

                    if (skill == null)
                    {
                        skill = new Skill { Name = skillDto.Name };
                        await _db.Skill.AddAsync(skill);
                        await _db.SaveChangesAsync();
                    }

                    _db.UserSkill.Add(new UserSkill
                    {
                        UserId = candidate.UserId,
                        SkillId = skill.SkillId,
                        YearsOfExperience = skillDto.Experience,
                        ProficiencyLevel = "begineer",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
            await _db.SaveChangesAsync();
        }
    }
}
