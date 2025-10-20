using Petrichor.Gateway.OpenApi;
using Petrichor.Gateway.Transforms;
using Petrichor.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");

builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer<OpenApiMerger>());

builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("http://localhost:4200")
            .WithExposedHeaders("Location"));
});

builder.Services.AddSingleton<AddAntiforgeryTokenResponseTransform>();
builder.Services.AddSingleton<ValidateAntiforgeryTokenRequestTransform>();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.ResponseTransforms
            .Add(builderContext.Services.GetRequiredService<AddAntiforgeryTokenResponseTransform>());

        builderContext.RequestTransforms
            .Add(builderContext.Services.GetRequiredService<ValidateAntiforgeryTokenRequestTransform>());
    })
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("API Reference");
        options.WithTheme(ScalarTheme.Mars);
    });
}

app.UseCors();

app.MapReverseProxy();

app.Run();