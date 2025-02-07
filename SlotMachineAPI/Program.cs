using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using SlotMachineAPI.Infrastructure.Context;
using SlotMachineAPI.Infrastructure.Repositories;
using SlotMachineAPI.Middleware;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Serilog Conf

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();

// MongoDB Conf
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDB"));

// Life Cycle Of DI Conf
builder.Services.AddSingleton<MongoDBContext>();
builder.Services.AddSingleton<IPlayerRepository, PlayerRepository>();

// MediatR Conf
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));


// Swagger Conf
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MongoDBTest API", Version = "v1" });
});

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MongoDBTest API v1");
        c.RoutePrefix = string.Empty;
    });
}
// Middleware Conf
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
