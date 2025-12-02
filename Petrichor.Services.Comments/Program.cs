using Petrichor.Services.Comments.Common.Extensions;
using Petrichor.ServiceDefaults;
using Petrichor.Services.Comments;
using Petrichor.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddWolverine();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();

builder.Services.AddEndpoints();
builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddCaching(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.ApplyMigrations();
}

app.UseStatusCodePages();
app.UseExceptionHandler();

app.MapEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.Run();