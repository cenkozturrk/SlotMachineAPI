using Microsoft.AspNetCore.Builder;
using SlotMachineAPI.Middleware;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Configures and applies custom middleware components to the application.
    /// This includes API documentation (Swagger) and global middleware such as logging and exception handling.
    /// Ensures middleware is correctly registered and executed in the request pipeline.
    /// </summary>
    /// <param name="app">The WebApplication instance to configure.</param>
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
