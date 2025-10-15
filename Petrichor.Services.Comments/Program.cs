using Petrichor.Services.Comments.Common.Extensions;
using Petrichor.ServiceDefaults;
using Petrichor.Services.Comments;

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

app.MapEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.Run();