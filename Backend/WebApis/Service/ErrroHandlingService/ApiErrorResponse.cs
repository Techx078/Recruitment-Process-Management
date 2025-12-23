    namespace WebApis.Service.ErrroHandlingService
    {
        public class ApiErrorResponse
        {
            public bool Success { get; set; } = false;
            public string Message { get; set; }
            public string ErrorCode { get; set; }
            public object? Errors { get; set; } // List<string> from validation
            public string? TraceId { get; set; }
        }
    }
