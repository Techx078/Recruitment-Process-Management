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


        }
    }
}
