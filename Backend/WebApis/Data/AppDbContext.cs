using Microsoft.EntityFrameworkCore;


namespace WebApis.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Skill> Skill { get; set; }
        public DbSet<UserSkill> UserSkill { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasOne(u => u.candidate)
                .WithOne(c => c.User)
                .HasForeignKey<Candidate>(c => c.UserId);
            base.OnModelCreating(modelBuilder);
            // UserSkill - cascade delete when Skill is deleted
            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.Skill)
                .WithMany(s => s.UserSkills)
                .HasForeignKey(us => us.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserSkill - no cascade delete when User is deleted
            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSkills)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
