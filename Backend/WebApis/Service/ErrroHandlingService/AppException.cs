namespace WebApis.Service.ErrorHandlingService
{
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }
        public object? Errors { get; }

        public AppException(
            string message,
            string errorCode,
            int statusCode,
            object? errors = null
        ) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Errors = errors;
        }
    }
}
