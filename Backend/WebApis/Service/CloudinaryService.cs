using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using WebApis.Service.ErrorHandlingService;
using WebApis.Service.ErrroHandlingService;

namespace WebApis.Service
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }
        public async Task<string> UploadResumeAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new AppException(
                    "No file uploaded.",
                    ErrorCodes.ValidationError,
                    StatusCodes.Status400BadRequest
                    );

            await using var stream = file.OpenReadStream();

            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "resumes", // Cloudinary folder
                PublicId = $"resume_{Guid.NewGuid()}"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception(result.Error.Message);

            return result.SecureUrl.ToString();
        }

        public async Task<string> UploadJobCandidateDocumentAsync(
            IFormFile file,
            int jobCandidateId,
            int jobDocumentId
        )
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is required");

            await using var stream = file.OpenReadStream();

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = $"job-candidates/{jobCandidateId}/documents",
                PublicId = $"jobDoc_{jobDocumentId}",
                Overwrite = true,
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception(result.Error.Message);

            return result.SecureUrl.ToString();
        }
    }
}
