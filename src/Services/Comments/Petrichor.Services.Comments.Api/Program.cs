using Petrichor.Services.Comments.Api.Common.Extensions;
using Petrichor.Services.Comments.Api;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

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

app.MapEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.Run();