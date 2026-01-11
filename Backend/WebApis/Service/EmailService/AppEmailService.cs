using WebApis.Data;
using WebApis.Dtos.MailDtos;

namespace WebApis.Service.EmailService
{
    public class AppEmailService : IAppEmailService
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        public AppEmailService(
            IEmailService emailService,
            IEmailTemplateService templateService)
        {
            _emailService = emailService;
            _templateService = templateService;
        }

        public async Task SendPasswordResetOtpAsync(User user, string otp)
        {
            var body = await _templateService.RenderAsync(
                "Auth/PasswordResetOtp.html",
                new Dictionary<string, string>
                {
                    ["USER_NAME"] = user.FullName,
                    ["OTP"] = otp
                });

            await _emailService.SendEmailAsync(
                user.Email,
                "Password Reset OTP",
                body
            );
        }

        public async Task SendCandidateRegistrationEmailAsync(
            User user,
            string plainPassword)
        {
            var body = await _templateService.RenderAsync(
                "Candidate/CandidateRegistered.html",
                new Dictionary<string, string>
                {
                    ["USER_NAME"] = user.FullName,
                    ["EMAIL"] = user.Email,
                    ["PASSWORD"] = plainPassword
                });

            await _emailService.SendEmailAsync(
                user.Email,
                "You have been registered on Roima portal",
                body
            );
        }

        public async Task SendBulkCandidateFailureReportAsync(
            string recruiterEmail,
            List<BulkCandidateFailureDto> failures)
        {
            var failureRows = string.Join("", failures.Select(f =>
                $"<tr><td>{f.Email}</td><td>{f.Reason}</td></tr>"
            ));

            var body = await _templateService.RenderAsync(
                "Recruiter/BulkCandidateFailureReport.html",
                new Dictionary<string, string>
                {
                    ["FAILED_COUNT"] = failures.Count.ToString(),
                    ["FAILURE_ROWS"] = failureRows
                });

            await _emailService.SendEmailAsync(
                recruiterEmail,
                "Bulk Candidate Registration – Failed Entries Report",
                body
            );
        }

        public async Task SendInternalUserRegistrationEmailAsync(
            User user,
            string password)
        {
            var body = await _templateService.RenderAsync(
                "InternalUser/UserRegistered.html",
                new Dictionary<string, string>
                {
                    ["FULL_NAME"] = user.FullName,
                    ["ROLE"] = user.RoleName,
                    ["EMAIL"] = user.Email,
                    ["PASSWORD"] = password
                });

            await _emailService.SendEmailAsync(
                user.Email,
                "Your ROIM Intelligence Account Has Been Created",
                body
            );
        }

        public async Task SendReviewerAssignedToJobEmailAsync(
            Reviewer reviewer,
            JobOpening jobOpening)
        {
            var body = await _templateService.RenderAsync(
                "Reviewer/AssignedToJob.html",
                new Dictionary<string, string>
                {
                    ["REVIEWER_NAME"] = reviewer.User.FullName,
                    ["JOB_TITLE"] = jobOpening.Title,
                    ["JOB_ID"] = jobOpening.Id.ToString()
                });

            await _emailService.SendEmailAsync(
                reviewer.User.Email,
                "You have been assigned as a Reviewer",
                body
            );
        }

        public async Task SendInterviewerAssignedToJobEmailAsync(
            Interviewer interviewer,
            JobOpening jobOpening)
        {
            var body = await _templateService.RenderAsync(
                "Interviewer/AssignedToJob.html",
                new Dictionary<string, string>
                {
                    ["INTERVIEWER_NAME"] = interviewer.User.FullName,
                    ["JOB_TITLE"] = jobOpening.Title,
                    ["JOB_ID"] = jobOpening.Id.ToString()
                });

            await _emailService.SendEmailAsync(
                interviewer.User.Email,
                "You have been assigned as an Interviewer",
                body
            );
        }

        public async Task SendCandidateAddedToJobEmailAsync(
            JobCandidateMailDto mailData)
        {
            var body = await _templateService.RenderAsync(
                "Candidate/AddedToJob.html",
                new Dictionary<string, string>
                {
                    ["CANDIDATE_NAME"] = mailData.CandidateName,
                    ["JOB_TITLE"] = mailData.JobTitle,
                    ["JOB_ID"] = mailData.JobOpeningId.ToString(),
                    ["RECRUITER_NAME"] = mailData.RecruiterName
                });

            await _emailService.SendEmailAsync(
                mailData.CandidateEmail,
                "You have been added to a job opportunity",
                body
            );
        }

        public async Task SendCandidateReviewResultEmailAsync(
            CandidateReviewResultMailDto mailData)
        {
            string templatePath = mailData.IsApproved
                ? "Candidate/ReviewApproved.html"
                : "Candidate/ReviewRejected.html";

            string subject = mailData.IsApproved
                ? "You’ve moved to the next round"
                : "Update on your job application";

            var body = await _templateService.RenderAsync(
                templatePath,
                new Dictionary<string, string>
                {
                    ["CANDIDATE_NAME"] = mailData.CandidateName,
                    ["JOB_TITLE"] = mailData.JobTitle,
                    ["REVIEWER_NAME"] = mailData.ReviewerName
                });

            await _emailService.SendEmailAsync(
                mailData.CandidateEmail,
                subject,
                body
            );
        }

        public async Task SendInterviewScheduledEmailAsync(
            CandidateInterviewScheduledMailDto mailData)
        {
            var body = await _templateService.RenderAsync(
                "Candidate/InterviewScheduled.html",
                new Dictionary<string, string>
                {
                    ["CANDIDATE_NAME"] = mailData.CandidateName,
                    ["JOB_TITLE"] = mailData.JobTitle,
                    ["INTERVIEW_TYPE"] = mailData.InterviewType,
                    ["ROUND_NUMBER"] = mailData.RoundNumber.ToString(),
                    ["INTERVIEW_DATE"] = mailData.InterviewDate
                        .ToString("dd MMM yyyy, hh:mm tt"),
                    ["MEETING_LINK"] = mailData.MeetingLink,
                    ["INTERVIEWER_NAME"] = mailData.InterviewerName
                });

            await _emailService.SendEmailAsync(
                mailData.CandidateEmail,
                $"Interview Scheduled – {mailData.JobTitle}",
                body
            );
        }

        public async Task SendInterviewFeedbackResultEmailAsync(
            InterviewFeedbackMailDto mailData)
        {
            string template;
            string subject;

            if (!mailData.IsPassed)
            {
                template = "Candidate/InterviewRejected.html";
                subject = $"Interview Result – {mailData.JobTitle}";
            }
            else if (mailData.NextStep == "Shortlisted")
            {
                template = "Candidate/InterviewShortlisted.html";
                subject = $"Congratulations! You’re Shortlisted – {mailData.JobTitle}";
            }
            else
            {
                template = "Candidate/InterviewNextRound.html";
                subject = $"Interview Update – {mailData.JobTitle}";
            }

            var body = await _templateService.RenderAsync(
                template,
                new Dictionary<string, string>
                {
                    ["CANDIDATE_NAME"] = mailData.CandidateName,
                    ["JOB_TITLE"] = mailData.JobTitle,
                    ["INTERVIEW_TYPE"] = mailData.InterviewType,
                    ["ROUND_NUMBER"] = mailData.RoundNumber.ToString(),
                    ["NEXT_STEP"] = mailData.NextStep
                });

            await _emailService.SendEmailAsync(
                mailData.CandidateEmail,
                subject,
                body
            );
        }

        public async Task SendOfferEmailAsync(OfferSentMailDto mailData)
        {
            var body = await _templateService.RenderAsync(
                "Candidate/OfferSent.html",
                new Dictionary<string, string>
                {
                    ["CANDIDATE_NAME"] = mailData.CandidateName,
                    ["JOB_TITLE"] = mailData.JobTitle,
                    ["OFFER_EXPIRY_DATE"] =
                        mailData.OfferExpiryDate.ToString("dd MMM yyyy")
                });

            await _emailService.SendEmailAsync(
                mailData.CandidateEmail,
                $"Offer Letter – {mailData.JobTitle}",
                body
            );
        }

        public async Task SendOfferRejectedBySystemEmailAsync(
            OfferRejectedBySystemMailDto mailData)
        {
            var body = await _templateService.RenderAsync(
                "Candidate/OfferRejectedBySystem.html",
                new Dictionary<string, string>
                {
                    ["CANDIDATE_NAME"] = mailData.CandidateName,
                    ["JOB_TITLE"] = mailData.JobTitle,
                    ["REJECTION_REASON"] = mailData.RejectionReason
                });

            await _emailService.SendEmailAsync(
                mailData.CandidateEmail,
                $"Update on Your Offer – {mailData.JobTitle}",
                body
            );
        }

        public async Task SendOfferExpiryExtendedEmailAsync(
            OfferExpiryExtendedMailDto mailData)
        {
            var body = await _templateService.RenderAsync(
                "Candidate/OfferExpiryExtended.html",
                new Dictionary<string, string>
                {
                    ["CANDIDATE_NAME"] = mailData.CandidateName,
                    ["JOB_TITLE"] = mailData.JobTitle,
                    ["NEW_EXPIRY_DATE"] =
                        mailData.NewExpiryDate.ToString("dd MMM yyyy")
                });

            await _emailService.SendEmailAsync(
                mailData.CandidateEmail,
                $"Offer Expiry Extended – {mailData.JobTitle}",
                body
            );
        }

        public async Task SendCandidateDocumentVerificationEmailAsync(
            CandidateDocumentVerificationMailDto mailData)
        {
            var template = mailData.IsVerified
                ? "Candidate/DocumentsVerified.html"
                : "Candidate/DocumentsRejected.html";

            var tokens = new Dictionary<string, string>
            {
                ["CANDIDATE_NAME"] = mailData.CandidateName,
                ["JOB_TITLE"] = mailData.JobTitle,
                ["REJECTION_REASON"] = mailData.RejectionReason ?? string.Empty
            };

            var body = await _templateService.RenderAsync(template, tokens);

            var subject = mailData.IsVerified
                ? $"Documents Verified – {mailData.JobTitle}"
                : $"Documents Rejected – Action Required";

            await _emailService.SendEmailAsync(
                mailData.CandidateEmail,
                subject,
                body
            );
        }


    }
}
