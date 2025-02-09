using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SlotMachineAPI.Application.Players.Commands.SpindCommand;
using SlotMachineAPI.Infrastructure.Context;
using SlotMachineAPI.Infrastructure.Repositories;
using SlotMachineAPI.Infrastructure.Service;
using System.Reflection;
using System.Text;

public static class ServiceExtensions
{
    /// <summary>
    /// Configures and registers all application-wide services.
    /// This includes logging, database configurations, dependency injection, authentication, validation etc. .
    /// Ensures all required services are properly initialized at application startup.
    /// </summary>
    /// <param name="services">The service collection to which dependencies are added.</param>
    /// <param name="configuration">Application configuration settings.</param>

    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Serilog Configuration
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration).CreateLogger();
        services.AddSingleton(Log.Logger);

        // MongoDB Configuration
        services.Configure<MongoDBSettings>(configuration.GetSection("MongoDB"));
        services.AddSingleton<MongoDBContext>();
        services.AddSingleton<IPlayerRepository, PlayerRepository>();

        // MediatR Configuration
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation Configuration
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<SpinCommandValidator>();

        // Swagger Configuration
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter your Format must be: Bearer [space ] -token-",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
        });

        // Auth Configuration
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<AuthService>();

        // Jtw Configuration
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "DefaultSecretKey");
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        services.AddControllers();
    }
}
