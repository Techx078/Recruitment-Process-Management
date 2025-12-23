using Microsoft.EntityFrameworkCore;
using WebApis.Data;
using WebApis.Dtos;

namespace WebApis.Repository.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        private readonly ICommonRepository<Education> _educationRepository;
        private readonly ICommonRepository<UserSkill> _userSkillRepository;
        private readonly ICommonRepository<Skill> _skillRepository;

        public UserRepository(
            AppDbContext db, 
            ICommonRepository<Education> educationRepository, 
            ICommonRepository<UserSkill> userSkillRepository, 
            ICommonRepository<Skill> skillRepository)
        {
            _db = db;
            _educationRepository = educationRepository;
            _userSkillRepository = userSkillRepository;
            _skillRepository = skillRepository;
        }

        public async Task AddEducationsAsync(Candidate candidate, IEnumerable<EducationDto> educations)
        {
            if (educations == null || !educations.Any())
                return;

            var educationEntities = educations.Select(e => new Education
            {
                Candidate = candidate,
                Degree = e.Degree,
                University = e.University,
                College = e.College,
                PassingYear = e.PassingYear,
                Percentage = e.Percentage
            }).ToList();

            await _educationRepository.AddRangeAsync(educationEntities);
        }

        public async Task AddSkillsAsync(User user, IEnumerable<SkillsDto> skills)
        {
            if (skills == null || !skills.Any())
                return;

            var userSkills = new List<UserSkill>();

            foreach (var skillDto in skills)
            {
                var skillName = skillDto.Name.Trim().ToLower();

                var skill = await _skillRepository.GetByFilterAsync(
                    s => s.Name.ToLower() == skillName
                );

                if (skill == null)
                {
                    skill = new Skill
                    {
                        Name = skillDto.Name.Trim()
                    };

                    await _skillRepository.AddAsync(skill);
                }

                userSkills.Add(new UserSkill
                {
                    UserId = user.Id,
                    SkillId = skill.SkillId,
                    YearsOfExperience = skillDto.Experience,
                    ProficiencyLevel = "Beginner",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await _userSkillRepository.AddRangeAsync(userSkills);
        }
    }
}
