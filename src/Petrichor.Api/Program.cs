using System.Reflection;
using Petrichor.Modules.Shared.Application;
using Petrichor.Modules.Shared.Infrastructure;
using Petrichor.Modules.Gallery.Presentation;
using Petrichor.Modules.Users.Presentation;
using Petrichor.Api.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();

Assembly[] moduleApplicationAssemblies = [
    Petrichor.Modules.Gallery.Application.AssemblyMarker.Assembly,
    Petrichor.Modules.Users.Application.AssemblyMarker.Assembly
];

builder.Services.AddApplication(moduleApplicationAssemblies);
builder.Services.AddInfrastructure(
    builder.Configuration,
    [
        GalleryModule.ConfigureConsumers
    ]
);

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
