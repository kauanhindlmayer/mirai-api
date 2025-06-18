var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("mirai-db")
    .WithImage("pgvector/pgvector")
    .WithImageTag("0.8.0-pg17")
    .WithDataVolume()
    .WithPgAdmin();

var redis = builder.AddRedis("mirai-redis");

var keycloak = builder.AddKeycloak("mirai-idp")
    .WithImageTag("26.2")
    .WithDataVolume()
    .WithRealmImport("../../.files/mirai-realm-export.json");

builder.AddProject<Projects.Presentation>("mirai-api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(redis)
    .WithReference(keycloak);

builder.AddDockerfile("mirai-nlp-api", "../../../mirai-nlp-api", "Dockerfile");

builder.Build().Run();