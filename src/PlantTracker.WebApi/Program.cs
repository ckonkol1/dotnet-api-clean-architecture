using Asp.Versioning;
using PlantTracker.Application.DependencyInjection;
using PlantTracker.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.UnsupportedApiVersionStatusCode = 400;
}).AddMvc();

builder.Services.AddControllers();
builder.Services.RegisterInfrastructure(builder.Configuration)
    .RegisterApplication();

var app = builder.Build();
app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();