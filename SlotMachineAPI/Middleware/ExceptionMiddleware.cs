using Serilog;
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
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = statusCode == 404 ? "The source could not be found." : "There was a server error, please try again!",
                Error = exception.Message
            };

            Log.Error(exception, "The global error was caught! StatusCode: {StatusCode} | Message: {Message}",
            statusCode, errorResponse.Message);

            response.StatusCode = statusCode;
            return response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}