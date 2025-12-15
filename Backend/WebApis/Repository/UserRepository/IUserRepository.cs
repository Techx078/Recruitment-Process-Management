using WebApis.Data;

namespace WebApis.Repository.UserRepository
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);
    }

}
