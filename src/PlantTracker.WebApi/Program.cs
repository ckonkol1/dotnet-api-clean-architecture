using Asp.Versioning;
using Microsoft.OpenApi.Models;
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
builder.Services.RegisterInfrastructure(builder.Configuration)
    .RegisterApplication();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "PlantTrack.WebAPI",
            Version = "v1"
        });
});

var app = builder.Build();

app.MapControllers();
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseAuthorization();


app.Run();