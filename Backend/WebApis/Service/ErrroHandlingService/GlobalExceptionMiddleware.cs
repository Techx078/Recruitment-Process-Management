using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebApis.Service.ErrroHandlingService;

namespace WebApis.Service.ErrorHandlingService
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
                var traceId = context.TraceIdentifier;
                _logger.LogError(ex, "Unhandled exception | TraceId: {TraceId}", traceId);

                await HandleExceptionAsync(context, ex, traceId);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            string traceId)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiErrorResponse
            {
                TraceId = traceId
            };

            switch (exception)
            {
                case AppException appEx:
                    context.Response.StatusCode = appEx.StatusCode;
                    response.Message = appEx.Message;
                    response.ErrorCode = appEx.ErrorCode;
                    response.Errors = appEx.Errors;
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "You are not authorized.";
                    response.ErrorCode = ErrorCodes.Unauthorized;
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = exception.Message;
                    response.ErrorCode = ErrorCodes.NotFound;
                    break;

                case TimeoutException:
                    context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
                    response.Message = "The request took too long. Please try again.";
                    response.ErrorCode = ErrorCodes.Timeout;
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "An unexpected error occurred. Please try later.";
                    response.ErrorCode = ErrorCodes.ServerError;
                    break;
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
