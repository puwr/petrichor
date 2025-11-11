var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("pg")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("database");

var cache = builder.AddRedis("cache");

var rmq = builder.AddRabbitMQ("rmq")
    .WithLifetime(ContainerLifetime.Persistent);

var minio = builder.AddMinioContainer("minio")
    .WithLifetime(ContainerLifetime.Persistent);

var users = builder.
    AddProject<Projects.Petrichor_Services_Users>("usersservice")
    .WaitFor(database)
    .WaitFor(cache)
    .WaitFor(rmq)
    .WithReference(database)
    .WithReference(cache)
    .WithReference(rmq);

var gallery = builder
    .AddProject<Projects.Petrichor_Services_Gallery>("galleryservice")
    .WaitFor(database)
    .WaitFor(cache)
    .WaitFor(rmq)
    .WaitFor(minio)
    .WithReference(database)
    .WithReference(cache)
    .WithReference(rmq)
    .WithReference(minio);

var comments = builder
    .AddProject<Projects.Petrichor_Services_Comments>("commentsservice")
    .WaitFor(database)
    .WaitFor(cache)
    .WaitFor(rmq)
    .WithReference(database)
    .WithReference(cache)
    .WithReference(rmq);

builder.AddProject<Projects.Petrichor_Gateway>("gateway")
    .WithReference(users)
    .WithReference(gallery)
    .WithReference(comments)
    .WaitFor(users)
    .WaitFor(gallery)
    .WaitFor(comments)
    .WithExternalHttpEndpoints();

builder.Build().Run();