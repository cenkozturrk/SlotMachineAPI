using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.UseCustomMiddlewares();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();