using Petrichor.ServiceDefaults;
using Petrichor.Services.Gallery;
using Petrichor.Services.Gallery.Common.Extensions;
using Petrichor.Services.Gallery.Common.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.AddApplication();
builder.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.ApplyMigrations();
}

app.UseExceptionHandler();

app.UseMinioStaticFiles();

app.MapEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.Run();