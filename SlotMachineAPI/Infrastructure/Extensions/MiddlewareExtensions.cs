using Microsoft.AspNetCore.Builder;
using SlotMachineAPI.Middleware;

public static class MiddlewareExtensions
{
    public static void UseCustomMiddlewares(this WebApplication app)
    {
        // Swagger Middleware 

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "MongoDBTest API v1");
            c.RoutePrefix = string.Empty;
        });

        // Special Middlewares
        app.UseMiddleware<LoggingMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
