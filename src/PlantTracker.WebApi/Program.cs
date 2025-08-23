using Asp.Versioning;
using PlantTracker.Application.DependencyInjection;
using PlantTracker.Infrastructure.DependencyInjection;
using PlantTracker.WebApi.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
        options.UnsupportedApiVersionStatusCode = 400;
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1);
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddControllers();
builder.Services
    .RegisterInfrastructure(builder.Configuration)
    .RegisterApplication()
    .AddOpenApi()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails();

var app = builder.Build();
app.UseStatusCodePages();
app.UseExceptionHandler();

app.MapControllers();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("PlantTracker.WebApi");
        options.WithTheme(ScalarTheme.DeepSpace);
        options.WithSidebar(false);
    });
}

//app.UseAuthorization();


app.Run();