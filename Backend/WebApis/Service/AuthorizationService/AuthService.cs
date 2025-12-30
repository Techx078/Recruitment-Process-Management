using System.Security.Claims;
using WebApis.Data;
using WebApis.Repository;
using WebApis.Repository.JobOpeningRepository;
using WebApis.Service.ErrorHandlingService;
using WebApis.Service.ErrroHandlingService;

namespace WebApis.Service.AuthorizationService
{
    public class AuthService : IAuthService
    {
        private readonly ICommonRepository<JobOpening> _jobOpeningRepository;
        private readonly ICommonRepository<Interviewer> _interviewerRepository;

        public AuthService(
            ICommonRepository<JobOpening> jobOpeningRepository,
            ICommonRepository<Interviewer> interviewerRepository)
        {
            _jobOpeningRepository = jobOpeningRepository;
            _interviewerRepository = interviewerRepository;
        }

        public async Task ValidateHrLevelAccessAsync(int jobOpeningId, ClaimsPrincipal user)
        {
            if (!await _jobOpeningRepository.ExistsAsync(j => j.Id == jobOpeningId))
                throw new AppException("Job opening not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleClaim = user.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                throw new UnauthorizedAccessException("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim);

            if (roleClaim == "Admin")
                return;

            if (roleClaim == "Interviewer")
            {
                var hrId = await _interviewerRepository.GetByFilterAsync(
                    i => i.UserId == loggedInUserId && i.Department == "HR",
                    i => i.Id
                );

                if (hrId == 0)
                    throw new AppException("HR profile not found", ErrorCodes.NotFound, StatusCodes.Status404NotFound);

                var isAssigned = await _jobOpeningRepository.ExistsAsync(
                    j => j.Id == jobOpeningId &&
                         j.JobInterviewers.Any(i =>
                             i.InterviewerId == hrId &&
                             i.Interviewer.Department == "HR")
                );

                if (!isAssigned)
                    throw new AppException(
                        "You are not assigned to this job opening",
                        ErrorCodes.Forbidden,
                        StatusCodes.Status403Forbidden
                    );

                return;
            }

            if (roleClaim == "Recruiter")
            {
                var isOwner = await _jobOpeningRepository.ExistsAsync(
                    j => j.Id == jobOpeningId &&
                         j.CreatedBy.UserId == loggedInUserId
                );

                if (!isOwner)
                    throw new AppException(
                        "You are not authorized to access this technical pool.",
                        ErrorCodes.Forbidden,
                        StatusCodes.Status403Forbidden
                    );

                return;
            }

            throw new AppException("Access denied", ErrorCodes.Forbidden, StatusCodes.Status403Forbidden);
        }
    }

}
