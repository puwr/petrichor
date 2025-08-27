using System.Reflection;
using Petrichor.Modules.Shared.Application;
using Petrichor.Modules.Shared.Infrastructure;
using Petrichor.Modules.Gallery.Presentation;
using Petrichor.Modules.Users.Presentation;
using Petrichor.Api.Extensions;
using Petrichor.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();

builder.Configuration.AddModuleConfiguration(["users"]);

Assembly[] moduleApplicationAssemblies = [
    Petrichor.Modules.Gallery.Application.AssemblyMarker.Assembly,
    Petrichor.Modules.Users.Application.AssemblyMarker.Assembly
];

builder.AddApplication(moduleApplicationAssemblies);
builder.AddInfrastructure(
    [
        GalleryModule.ConfigureConsumers
    ]
);

builder.AddUsersModule();
builder.AddGalleryModule();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.ApplyMigrations();
}

app.UseExceptionHandler();

app.UseInfrastructureMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
