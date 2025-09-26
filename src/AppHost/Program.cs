var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("pgvector/pgvector")
    .WithImageTag("0.8.0-pg17")
    .WithDataVolume()
    .WithPgAdmin();

var database = postgres.AddDatabase("mirai-db");

var redis = builder.AddRedis("redis")
    .WithRedisInsight();

var keycloak = builder.AddKeycloak("keycloak", port: 8080)
    .WithImageTag("26.2")
    .WithDataVolume()
    .WithRealmImport("../../.files/mirai-realm-export.json");

var ollama = builder.AddOllama("ollama")
    .WithDataVolume()
    .WithContainerRuntimeArgs("--gpus=all");

var llama = ollama.AddModel("chat", "llama3.2:1b");
var minilm = ollama.AddModel("embedding", "all-minilm");

var miraiApi = builder.AddProject<Projects.Presentation>("mirai-api")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(redis)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithReference(llama)
    .WithReference(minilm)
    .WithExternalHttpEndpoints();

builder.AddNpmApp("mirai-app", "../../../mirai-app")
    .WithReference(miraiApi)
    .WaitFor(miraiApi)
    .WithHttpEndpoint(env: "PORT", port: 5173)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();