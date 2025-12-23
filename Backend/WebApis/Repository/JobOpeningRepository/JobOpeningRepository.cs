using WebApis.Data;
using WebApis.Dtos.JobOpeningDto;

namespace WebApis.Repository.JobOpeningRepository
{
    public class JobOpeningRepository : IJobOpeningRepository
    {
        private readonly AppDbContext _db;
        private readonly ICommonRepository<Skill> _skillRepository;
        public JobOpeningRepository(
            AppDbContext db,
            ICommonRepository<Skill> skillRepository
            ) {
            _db = db;
            _skillRepository = skillRepository;
        }
        public async Task AddDocumentsAsync(int jobId, IEnumerable<jobDocumentDto> documents)
        {
            if (documents == null || !documents.Any()) return;

            var docs = documents.Select(doc => new JobDocument
            {
                JobOpeningId = jobId,
                DocumentId = doc.DocumentId,
                IsMandatory = doc.IsMandatory
            });

            await _db.JobDocuments.AddRangeAsync(docs);
            await _db.SaveChangesAsync();
        }

        public async Task AddInterviewersAsync(int jobId, IEnumerable<int> interviewerIds)
        {
            if (interviewerIds == null || !interviewerIds.Any()) return;

            var interviewers = interviewerIds.Select(id => new JobInterviewer
            {
                JobOpeningId = jobId,
                InterviewerId = id
            });

            await _db.jobInterviewer.AddRangeAsync(interviewers);
            await _db.SaveChangesAsync();
        }

        public async Task AddJobSkillsAsync(int jobId, IEnumerable<jobSkillDto> skills)
        {
            if (skills == null || !skills.Any()) return;

            foreach (var skill in skills)
            {
                var skillTemp = await _skillRepository.GetByFilterAsync(
                    s => s.Name.ToLower() == skill.SkillName.Trim().ToLower()
                );

                if (skillTemp == null)
                {
                    skillTemp = new Skill { Name = skill.SkillName.Trim() };
                    await _skillRepository.AddAsync(skillTemp);
                }

                await _db.jobSkills.AddAsync(new JobSkill
                {
                    JobOpeningId = jobId,
                    SkillId = skillTemp.SkillId,
                    IsRequired = skill.IsRequired,
                    minExperience = skill.minExperience
                });
            }
            await _db.SaveChangesAsync();
        }
        public async Task AddReviewersAsync(int jobId, IEnumerable<int> reviewerIds)
        {
            if (reviewerIds == null || !reviewerIds.Any()) return;

            var reviewers = reviewerIds.Select(id => new JobReviewer
            {
                JobOpeningId = jobId,
                ReviewerId = id
            });

            await _db.JobReviewer.AddRangeAsync(reviewers);
            await _db.SaveChangesAsync();
        }
    }
}
