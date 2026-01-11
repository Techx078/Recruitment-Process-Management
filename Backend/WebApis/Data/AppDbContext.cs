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
        public DbSet<JobOpening> JobOpening { get; set; }
        public DbSet<JobInterviewer> jobInterviewer { get; set; }
        public DbSet<JobReviewer> JobReviewer { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }  
        public DbSet<Interviewer> Interviewers { get; set; }
        public DbSet<Recruiter> Recruiter { get; set; }
        public DbSet<JobCandidate> JobCandidate { get; set; }
        public DbSet<JobInterview> JobInterview { get; set; }
        public DbSet<JobDocument> JobDocuments { get; set; }
        public DbSet<Document> Documents { get; set; }

        public DbSet<JobSkill> jobSkills { get; set; }

        public DbSet<PasswordReset> PasswordResets { get; set; }

        public DbSet<Education> Educations { get; set; }

        public DbSet<JobCandidateDocus> jobCandidateDocs { get; set; }

        public DbSet<Employee> Employees { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //document seed
            modelBuilder.Entity<Document>().HasData(
                       new Document
                       {
                           id = 1,
                           Name = "Addhar-card",
                           Description = "Need both side",
                           CreateAt = DateTime.UtcNow
                       },
                       new Document
                       {
                           id = 2,
                           Name = "Offer Letter",
                           Description = "Job Offer Document",
                           CreateAt = DateTime.UtcNow
                       },
                       new Document
                       {
                           id = 3,
                           Name = "ID Proof",
                           Description = "Government Issued ID",
                           CreateAt = DateTime.UtcNow
                       }
                   );

            // Skill Seed
            modelBuilder.Entity<Skill>().HasData(
                new Skill
                {
                    SkillId = 1,
                    Name = "C#",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Skill
                {
                    SkillId = 2,
                    Name = "ASP.NET Core",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Skill
                {
                    SkillId = 3,
                    Name = "React",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Skill
                {
                    SkillId = 4,
                    Name = "SQL Server",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
    );
            modelBuilder.Entity<User>().HasData(
                    new User
                    {
                        Id = 1, 
                        FullName = "Admin User",
                        Email = "admin@example.com",
                        PhoneNumber = "9999999999",
                        PasswordHash = "$2a$10$F9qsthzYRMDmWGKbSMmZ4.EzG80t9HJDGEsm1JXgbSF2GfXS5JQVu", //password :- Admin123
                        RoleName = "Admin",
                        CreatedAt = new DateTime(2025, 01, 01),
                        Domain = "not-defined",
                        DomainExperienceYears = 0
                    }
                );
            modelBuilder.Entity<JobCandidate>()
                        .HasIndex(j => new { j.JobOpeningId, j.CandidateId })
                        .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Domain)
                .HasConversion<string>();   
        modelBuilder.Entity<Education>()
            .HasOne(e => e.Candidate)
            .WithMany(c => c.Educations)
            .HasForeignKey(e => e.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<PasswordReset>()
                .HasKey(pr => pr.Id);
                
            // JobSkill - cascade delete when Skill is deleted
            modelBuilder.Entity<JobSkill>()
                .HasOne(js => js.Skill)
                .WithMany(s => s.JobSkills)
                .HasForeignKey(js => js.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserSkill - no cascade delete when User is deleted 
            modelBuilder.Entity<JobSkill>()
                .HasOne(js => js.JobOpening)
                .WithMany(u => u.JobSkills)
                .HasForeignKey(us => us.JobOpeningId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
            .Entity<JobOpening>()
           .Property(j => j.Status)
           .HasConversion<string>();

            modelBuilder
                .Entity<JobOpening>()
                .Property(j => j.JobType)
                .HasConversion<string>();

            modelBuilder
                .Entity<JobOpening>()
                .Property(j => j.Department)
                .HasConversion<string>();

            modelBuilder
                .Entity<JobOpening>()
                .Property(j => j.Location)
                .HasConversion<string>();

            modelBuilder
                .Entity<JobOpening>()
                .Property(j => j.Education)
                .HasConversion<string>();
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


            modelBuilder.Entity<JobReviewer>()
            .HasOne(jr => jr.JobOpening)
            .WithMany(j => j.JobReviewers)
            .HasForeignKey(jr => jr.JobOpeningId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobReviewer>()
            .HasOne(jr => jr.Reviewer)
            .WithMany(r => r.JobReviewers)
            .HasForeignKey(jr => jr.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobInterviewer>()
               .HasOne(ji => ji.JobOpening)
               .WithMany(j => j.JobInterviewers)
               .HasForeignKey(ji => ji.JobOpeningId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobInterviewer>()
                .HasOne(ji => ji.Interviewer)
                .WithMany(i => i.JobInterviewers)
                .HasForeignKey(ji => ji.InterviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobCandidate>()
            .HasOne(jc => jc.JobOpening)
            .WithMany(j => j.JobCandidates)
            .HasForeignKey(jc => jc.JobOpeningId)
            .OnDelete(DeleteBehavior.Cascade); // if Job deleted → related JobCandidates deleted

            modelBuilder.Entity<JobCandidate>()
                .HasOne(jc => jc.Candidate)
                .WithMany(c => c.JobCandidates)
                .HasForeignKey(jc => jc.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobInterview>()
            .HasOne(ji => ji.JobCandidate)
            .WithMany(jc => jc.JobInterviews)
            .HasForeignKey(ji => ji.JobCandidateId)
            .OnDelete(DeleteBehavior.Cascade); // if JobCandidate deleted → related JobInterviews deleted

            modelBuilder.Entity<JobInterview>()
                .HasOne(ji => ji.Interviewer)
                .WithMany(i => i.JobInterviews)
                .HasForeignKey(ji => ji.InterviewerId)
                .OnDelete(DeleteBehavior.Restrict); // keep interviewer's history

            modelBuilder.Entity<JobCandidateDocus>()
                  .HasOne(jcd => jcd.JobCandidate)
                  .WithMany(c => c.JobCandidateDoc) 
                  .HasForeignKey(jcd => jcd.JobCandidateId)
                  .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobCandidateDocus>()
                .HasOne(jcd => jcd.JobDocument)
                .WithMany(d => d.JobCandidateDoc) 
                .HasForeignKey(jcd => jcd.JobDocumentId)
                .OnDelete(DeleteBehavior.Cascade);
            



            base.OnModelCreating(modelBuilder);


        }
    }
}
