using System.Reflection;
using Petrichor.Shared.Application;
using Petrichor.Shared.Infrastructure;
using Petrichor.Modules.Gallery.Presentation;
using Petrichor.Modules.Users.Presentation;
using Petrichor.Api.Extensions;
using Scalar.AspNetCore;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();

builder.Services.AddCors();

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

Assembly[] moduleApplicationAssemblies = [
    Petrichor.Modules.Gallery.Application.AssemblyMarker.Assembly,
    Petrichor.Modules.Users.Application.AssemblyMarker.Assembly
];

builder.Services.AddApplication(moduleApplicationAssemblies);
builder.Services.AddInfrastructure();

builder.Configuration.AddModuleConfiguration(["users"]);

builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddGalleryModule(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("API Reference");
        options.WithTheme(ScalarTheme.Mars);
    });

    app.ApplyMigrations();
}

app.UseExceptionHandler();

app.UseInfrastructureMiddleware();

app.UseCors(x => x.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("http://localhost:4200")
    .WithExposedHeaders("Location"));

app.UseAuthentication();
app.UseAuthorization();

app.UseGalleryMiddleware();

app.MapControllers();

app.Run();
