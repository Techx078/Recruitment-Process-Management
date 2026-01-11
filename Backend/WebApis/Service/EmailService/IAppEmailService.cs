using WebApis.Data;
using WebApis.Dtos.MailDtos;

namespace WebApis.Service.EmailService
{
    public interface IAppEmailService
    {
        Task SendPasswordResetOtpAsync(User user, string otp);

        Task SendCandidateRegistrationEmailAsync(
           User user,
           string plainPassword
        );
        Task SendBulkCandidateFailureReportAsync(
          string recruiterEmail,
          List<BulkCandidateFailureDto> failures
        );
        Task SendInternalUserRegistrationEmailAsync(
              User user,
              string password
        );

        Task SendReviewerAssignedToJobEmailAsync(
            Reviewer reviewer,
            JobOpening jobOpening
        );

        Task SendInterviewerAssignedToJobEmailAsync(
            Interviewer interviewer,
            JobOpening jobOpening
        );

        Task SendCandidateAddedToJobEmailAsync(JobCandidateMailDto mailData);

        Task SendCandidateReviewResultEmailAsync(
            CandidateReviewResultMailDto mailData);

        Task SendInterviewScheduledEmailAsync(
            CandidateInterviewScheduledMailDto mailData);

        Task SendInterviewFeedbackResultEmailAsync(
            InterviewFeedbackMailDto mailData);

        Task SendOfferEmailAsync(OfferSentMailDto mailData);

        Task SendOfferRejectedBySystemEmailAsync(
            OfferRejectedBySystemMailDto mailData);

        Task SendOfferExpiryExtendedEmailAsync(
            OfferExpiryExtendedMailDto mailData);

        Task SendCandidateDocumentVerificationEmailAsync(
            CandidateDocumentVerificationMailDto mailData);

        Task SendCandidateJoiningDateEmailAsync(
            CandidateJoiningDateMailDto mailData);
    }
}
