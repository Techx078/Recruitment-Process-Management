using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApis.Data;
using WebApis.Dtos.JobCandidateDtos;
using WebApis.Dtos.MailDtos;

namespace WebApis.Repository.JobCandidateRepository
{
    public class JobCandidateRepository : IJobCandidateRepository
    {
        private readonly AppDbContext _db;

        public JobCandidateRepository(AppDbContext db) {
            _db = db;
        }

        public async Task<JobCandidate> CreateJobCandidate(JobCandidateCreateDto dto)
        {
            var JobCandidate = new JobCandidate
            {
                JobOpeningId = dto.JobOpeningId,
                CandidateId = dto.CandidateId,
                CvPath = dto.CvPath,
                Status = Status.Applied.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _db.JobCandidate.AddAsync(JobCandidate);
            await _db.SaveChangesAsync();
            return JobCandidate;
        }

        public async Task<JobCandidateMailDto> GetCandidateJobMailData(int jobCandidateId)
        {
            return await _db.JobCandidate
                .Where(jc => jc.Id == jobCandidateId)
                .Select(jc => new JobCandidateMailDto
                {
                    CandidateName = jc.Candidate.User.FullName,
                    CandidateEmail = jc.Candidate.User.Email,
                    JobTitle = jc.JobOpening.Title,
                    JobOpeningId = jc.JobOpening.Id,
                    RecruiterName = jc.JobOpening.CreatedBy.User.FullName
                })
                .FirstAsync();
        }

        public async Task<CandidateReviewResultMailDto> GetCandidateReviewMailData(
            int jobCandidateId)
        {
            return await _db.JobCandidate
                .Where(jc => jc.Id == jobCandidateId)
                .Select(jc => new CandidateReviewResultMailDto
                {
                    CandidateName = jc.Candidate.User.FullName,
                    CandidateEmail = jc.Candidate.User.Email,
                    JobTitle = jc.JobOpening.Title,
                    ReviewerName = jc.Reviewer.User.FullName,
                    IsApproved = jc.Status == "Reviewed"
                })
                .FirstAsync();
        }

        public async Task<CandidateInterviewScheduledMailDto> GetCandidateInterviewScheduledMailData(int jobInterviewId)
        {
            return await _db.JobInterview
                .Where(i => i.Id == jobInterviewId)
                .Select(i => new CandidateInterviewScheduledMailDto
                {
                    CandidateName = i.JobCandidate.Candidate.User.FullName,
                    CandidateEmail = i.JobCandidate.Candidate.User.Email,

                    JobTitle = i.JobCandidate.JobOpening.Title,
                    InterviewType = i.InterviewType,
                    RoundNumber = i.RoundNumber,

                    InterviewDate = i.ScheduledAt,
                    MeetingLink = i.MeetingLink,

                    InterviewerName = i.Interviewer.User.FullName
                })
                .FirstAsync();
        }

        public async Task<InterviewFeedbackMailDto>GetInterviewFeedbackMailData(int jobCandidateId)
        {
            return await _db.JobCandidate
                .Where(c => c.Id == jobCandidateId)
                .Select(c => new InterviewFeedbackMailDto
                {
                    CandidateName = c.Candidate.User.FullName,
                    CandidateEmail = c.Candidate.User.Email,

                    JobTitle = c.JobOpening.Title,
                    InterviewType = c.JobInterviews
                        .OrderByDescending(i => i.UpdatedAt)
                        .Select(i => i.InterviewType)
                        .First(),

                    RoundNumber = c.RoundNumber,
                    IsPassed = c.Status != "Rejected",
                    NextStep = c.Status
                })
                .FirstAsync();
        }

        public async Task<OfferSentMailDto> GetOfferSentMailData(int jobCandidateId)
        {
            return await _db.JobCandidate
                .Where(c => c.Id == jobCandidateId)
                .Select(c => new OfferSentMailDto
                {
                    CandidateName = c.Candidate.User.FullName,
                    CandidateEmail = c.Candidate.User.Email,
                    JobTitle = c.JobOpening.Title,
                    OfferExpiryDate = c.OfferExpiryDate
                })
                .FirstAsync();
        }

        public async Task<OfferRejectedBySystemMailDto>GetOfferRejectedBySystemMailData(int jobCandidateId)
        {
            return await _db.JobCandidate
                .Where(c => c.Id == jobCandidateId)
                .Select(c => new OfferRejectedBySystemMailDto
                {
                    CandidateName = c.Candidate.User.FullName,
                    CandidateEmail = c.Candidate.User.Email,
                    JobTitle = c.JobOpening.Title,
                    RejectionReason = c.OfferRejectionReason != null ? 
                                        c.OfferRejectionReason 
                                        : "offer expired"
                })
                .FirstAsync();
        }

        public async Task<OfferExpiryExtendedMailDto>
        GetOfferExpiryExtendedMailData(int jobCandidateId)
        {
            return await _db.JobCandidate
                .Where(c => c.Id == jobCandidateId)
                .Select(c => new OfferExpiryExtendedMailDto
                {
                    CandidateName = c.Candidate.User.FullName,
                    CandidateEmail = c.Candidate.User.Email,
                    JobTitle = c.JobOpening.Title,
                    NewExpiryDate = c.OfferExpiryDate
                })
                .FirstAsync();
        }

        public async Task<CandidateDocumentVerificationMailDto> GetCandidateDocumentVerificationMailData(int jobCandidateId)
        {
            return await _db.JobCandidate
                .Where(c => c.Id == jobCandidateId)
                .Select(c => new CandidateDocumentVerificationMailDto
                {
                    CandidateName = c.Candidate.User.FullName,
                    CandidateEmail = c.Candidate.User.Email,
                    JobTitle = c.JobOpening.Title,
                    IsVerified = c.IsDocumentVerified,
                    RejectionReason = c.DocumentUnVerificationReason
                })
                .FirstAsync();
        }

        public async Task<CandidateJoiningDateMailDto> GetCandidateJoiningDateMailData(int jobCandidateId)
        {
            return await _db.JobCandidate
                .Where(c => c.Id == jobCandidateId)
                .Select(c => new CandidateJoiningDateMailDto
                {
                    CandidateName = c.Candidate.User.FullName,
                    CandidateEmail = c.Candidate.User.Email,
                    JobTitle = c.JobOpening.Title,
                    JoiningDate = c.JoiningDate
                })
                .FirstAsync();
        }

        public async Task<EmployeeCreatedMailDto>GetEmployeeCreatedMailData(int jobCandidateId)
        {
            return await _db.JobCandidate
                .Where(c => c.Id == jobCandidateId)
                .Select(c => new EmployeeCreatedMailDto
                {
                    EmployeeName = c.Candidate.User.FullName,
                    EmployeeEmail = c.Candidate.User.Email,
                    Department = c.JobOpening.Department,
                    Designation = c.JobOpening.Domain,
                    JoiningDate = c.JoiningDate!.Value
                })
                .FirstAsync();
        }

    }
}
