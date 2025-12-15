using Microsoft.EntityFrameworkCore;
using WebApis.Data;

namespace WebApis.Repository.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email == email.ToLower());
        }
    }
}
