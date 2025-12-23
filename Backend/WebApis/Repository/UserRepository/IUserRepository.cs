using WebApis.Data;
using WebApis.Dtos;

namespace WebApis.Repository.UserRepository
{
    public interface IUserRepository
    {
        Task AddEducationsAsync(Candidate candidate, IEnumerable<EducationDto> educations);
        Task AddSkillsAsync(User user, IEnumerable<SkillsDto> skills);

    }

}
