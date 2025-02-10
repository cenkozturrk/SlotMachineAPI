using Serilog;
using SlotMachineAPI.Application.Enums;
using System.Net;
using System.Text.Json;

namespace SlotMachineAPI.Middleware
{
    /// <summary>
    /// Middleware for handling global exceptions.
    /// Logs errors and returns user-friendly error messages.
    /// </summary>

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Global error was caught! Request: {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var statusCode = exception switch
            {
                UnauthorizedAccessException => (int)ErrorMessages.Unauthorized,
                ArgumentException => (int)ErrorMessages.BadRequest,
                KeyNotFoundException => (int)ErrorMessages.NotFound,
                _ => (int)ErrorMessages.ServerError
            };

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = ((ErrorMessages)statusCode).GetMessage(), // take message from enum
                Error = exception.Message
            };

            Log.Error(exception, "The global error was caught! StatusCode: {StatusCode} | Message: {Message}",
            statusCode, errorResponse.Message);

            response.StatusCode = statusCode;
            return response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}

