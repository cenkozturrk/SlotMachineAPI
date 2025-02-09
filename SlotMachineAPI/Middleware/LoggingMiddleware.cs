using System.Diagnostics;
using Serilog;

public class LoggingMiddleware
{
    /// <summary>
    /// Middleware for logging HTTP requests and responses.
    /// Measures execution time and logs any errors that occur during request processing.
    /// </summary>
    private readonly RequestDelegate _next;
    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;
        var requestBody = request.Path + (request.QueryString.HasValue ? request.QueryString.Value : "");

        Log.Information(" Request: {Method} {Path} | Body: {Body} | IP: {IP}",
            request.Method, request.Path, requestBody, context.Connection.RemoteIpAddress);

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, " Error has occured! Request: {Method} {Path}", request.Method, request.Path);
            throw;
        }

        stopwatch.Stop();
        Log.Information(" Response: {StatusCode} {Path} | Time: {Elapsed} ms",
            context.Response.StatusCode, request.Path, stopwatch.ElapsedMilliseconds);
    }
}
