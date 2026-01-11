using Microsoft.AspNetCore.Mvc;
using WebApis.Data;
using WebApis.Dtos.JobCandidateDtos;
using WebApis.Dtos.MailDtos;

namespace WebApis.Repository.JobCandidateRepository
{
    public interface IJobCandidateRepository
    {
        Task<JobCandidate> CreateJobCandidate(JobCandidateCreateDto dto);

        public Task<JobCandidateMailDto> GetCandidateJobMailData(int jobCandidateId);

        public Task<CandidateReviewResultMailDto> GetCandidateReviewMailData(
           int jobCandidateId);

        public Task<CandidateInterviewScheduledMailDto> GetCandidateInterviewScheduledMailData(int jobInterviewId);

        public Task<InterviewFeedbackMailDto> GetInterviewFeedbackMailData(int jobCandidateId);

        public Task<OfferSentMailDto> GetOfferSentMailData(int jobCandidateId);

        public Task<OfferRejectedBySystemMailDto> GetOfferRejectedBySystemMailData(int jobCandidateId);

        public Task<OfferExpiryExtendedMailDto> GetOfferExpiryExtendedMailData(int jobCandidateId);

        public Task<CandidateDocumentVerificationMailDto> GetCandidateDocumentVerificationMailData(int jobCandidateId);

        public Task<CandidateJoiningDateMailDto> GetCandidateJoiningDateMailData(int jobCandidateId);


    }
}
