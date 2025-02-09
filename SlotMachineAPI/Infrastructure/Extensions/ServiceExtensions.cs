using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.OpenApi.Models;
using Serilog;
using SlotMachineAPI.Application.Players.Commands.SpindCommand;
using SlotMachineAPI.Infrastructure.Context;
using SlotMachineAPI.Infrastructure.Repositories;
using SlotMachineAPI.Infrastructure.Service;
using System.Reflection;

public static class ServiceExtensions
{
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
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "MongoDBTest API", Version = "v1" });
        });

        // Auth Configuration
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<AuthService>();

        services.AddControllers();
    }
}
