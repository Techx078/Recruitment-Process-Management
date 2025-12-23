namespace WebApis.Service.ErrroHandlingService
{
    public class ErrorCodes
    {
        public const string ValidationError = "VALIDATION_ERROR";
        public const string Unauthorized = "UNAUTHORIZED";
        public const string Forbidden = "FORBIDDEN";
        public const string NotFound = "NOT_FOUND";
        public const string Conflict = "CONFLICT";
        public const string Duplicate = "DUPLICATE_ENTRY";
        public const string NoChanges = "NO_CHANGES";
        public const string DeleteConflict = "DELETE_CONFLICT";
        public const string Timeout = "REQUEST_TIMEOUT";
        public const string ServerError = "INTERNAL_SERVER_ERROR";
    }
}
