using Dutchskull.Aspire.PolyRepo;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("pgvector/pgvector")
    .WithImageTag("0.8.0-pg17")
    .WithDataVolume()
    .WithPgAdmin();

var redis = builder.AddRedis("redis");

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
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithReference(redis)
    .WithReference(keycloak)
    .WithReference(llama)
    .WithReference(minilm)
    .WithExternalHttpEndpoints();

var repository = builder.AddRepository(
    "mirai-repo",
    "https://github.com/kauanhindlmayer/mirai-app",
    config => config.WithDefaultBranch("main"));

builder.AddNpmAppFromRepository("mirai-app", repository, string.Empty)
    .WithReference(miraiApi)
    .WaitFor(miraiApi)
    .WithHttpEndpoint(env: "PORT", port: 5173)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();