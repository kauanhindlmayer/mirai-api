var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("mirai-db")
    .WithImage("pgvector/pgvector")
    .WithImageTag("0.8.0-pg17")
    .WithDataVolume()
    .WithPgAdmin();

var redis = builder.AddRedis("mirai-redis");

var keycloak = builder.AddKeycloak("mirai-idp", port: 8080)
    .WithImageTag("26.2")
    .WithDataVolume()
    .WithRealmImport("../../.files/mirai-realm-export.json");

var miraiApi = builder.AddProject<Projects.Presentation>("mirai-api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(redis)
    .WithReference(keycloak)
    .WithExternalHttpEndpoints();

builder.AddDockerfile("mirai-nlp-api", "../../../mirai-nlp-api", "Dockerfile")
    .WithHttpEndpoint(port: 8000, targetPort: 8000)
    .WithUrl("http://127.0.0.1:8000/docs");

builder.AddNpmApp("mirai-app", "../../../mirai-app")
    .WithReference(miraiApi)
    .WaitFor(miraiApi)
    .WithHttpEndpoint(env: "PORT", port: 5173)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();