var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("mirai-db")
    .WithImage("pgvector/pgvector")
    .WithImageTag("0.8.0-pg17")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

var redis = builder.AddRedis("mirai-redis")
    .WithLifetime(ContainerLifetime.Persistent);

var keycloak = builder.AddKeycloak("mirai-idp", port: 8080)
    .WithImageTag("26.2")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRealmImport("../../.files/mirai-realm-export.json");

builder.AddProject<Projects.WebApi>("mirai-api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(redis)
    .WithReference(keycloak)
    .WaitFor(keycloak);

builder.AddDockerfile("mirai-nlp-api", "../../../mirai-nlp-api", "Dockerfile")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("ENVIRONMENT", "development");

builder.Build().Run();