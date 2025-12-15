using System.Text.Json;
namespace WebApis.Service.ErrroHandlingService
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await HandleException(context, ex);
            }
        }

        private static async Task HandleException(
            HttpContext context,
            Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiErrorResponse();

            switch (exception)
            {
                case AppException appEx:
                    context.Response.StatusCode = appEx.StatusCode;
                    response.Message = appEx.Message;
                    response.ErrorCode = "BUSINESS_ERROR";
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Unauthorized access";
                    response.ErrorCode = "UNAUTHORIZED";
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = exception.Message;
                    response.ErrorCode = "NOT_FOUND";
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "Something went wrong. Please try again.";
                    response.ErrorCode = "INTERNAL_SERVER_ERROR";
                    break;
            }

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    
    }
}
