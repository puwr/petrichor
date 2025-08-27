var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("pg")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("database");

var rmq = builder.AddRabbitMQ("rmq")
    .WithLifetime(ContainerLifetime.Persistent);

var minio = builder.AddMinioContainer("minio")
    .WithLifetime(ContainerLifetime.Persistent);

var api = builder
    .AddProject<Projects.Petrichor_Api>("apiservice")
    .WaitFor(database)
    .WaitFor(rmq)
    .WaitFor(minio)
    .WithReference(database)
    .WithReference(rmq)
    .WithReference(minio);

var comments = builder
    .AddProject<Projects.Petrichor_Services_Comments_Api>("commentsservice")
    .WaitFor(database)
    .WaitFor(rmq)
    .WithReference(database)
    .WithReference(rmq);

builder.AddProject<Projects.Petrichor_Gateway>("gateway")
    .WithReference(api)
    .WithReference(comments)
    .WaitFor(api)
    .WaitFor(comments)
    .WithExternalHttpEndpoints();

builder.Build().Run();