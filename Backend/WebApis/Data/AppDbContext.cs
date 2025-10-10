using Microsoft.EntityFrameworkCore;

namespace WebApis.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
       
    }
}
