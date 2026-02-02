var builder = DistributedApplication.CreateBuilder(args);

var database = builder
    .AddPostgres("pg")
    .WithImage("postgres:18.1-alpine")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("database");

var cache = builder
    .AddRedis("cache")
    .WithImage("redis:8.4-alpine");

var rmq = builder
    .AddRabbitMQ("rmq")
    .WithImage("rabbitmq:4.2-alpine")
    .WithLifetime(ContainerLifetime.Persistent);

var minioUser = builder.AddParameter("MinioUser");
var minioPassword = builder.AddParameter("MinioPassword", secret: true);

var minio = builder
    .AddMinioContainer("minio", minioUser, minioPassword)
    .WithImage("minio/minio:RELEASE.2025-09-07T16-13-09Z")
    .WithLifetime(ContainerLifetime.Persistent);
minioUser.WithParentRelationship(minio);
minioPassword.WithParentRelationship(minio);

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