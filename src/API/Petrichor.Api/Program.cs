using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Petrichor.Shared.Application;
using Petrichor.Shared.Infrastructure;
using Petrichor.Modules.Gallery.Presentation;
using Petrichor.Modules.Users.Presentation;
using Petrichor.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();

builder.Services.AddCors();

builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
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
