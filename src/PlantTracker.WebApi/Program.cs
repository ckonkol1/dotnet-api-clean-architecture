using PlantTracker.Application.DependencyInjection;
using PlantTracker.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.RegisterInfrastructure(builder.Configuration)
    .RegisterApplication();

var app = builder.Build();
app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();