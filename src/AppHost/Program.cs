var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("pgvector/pgvector")
    .WithImageTag("0.8.0-pg17")
    .WithDataVolume()
    .WithPgAdmin(pgAdmin => pgAdmin.WithExplicitStart());

var database = postgres.AddDatabase("mirai-db");

var redis = builder.AddRedis("redis")
    .WithRedisInsight(redisInsight => redisInsight.WithExplicitStart());

var keycloak = builder.AddKeycloak("keycloak", port: 8080)
    .WithImageTag("26.2")
    .WithDataVolume()
    .WithRealmImport("../../.files/mirai-realm-export.json");

var keycloakBaseUrl = keycloak.GetEndpoint("http").Property(EndpointProperty.Url);

var openai = builder.AddOpenAI("openai");

var chat = openai.AddModel("chat", "gpt-4o-mini");
var embeddings = openai.AddModel("embeddings", "text-embedding-3-small");

var miraiApi = builder.AddProject<Projects.Presentation>("mirai-api")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(redis)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithReference(chat)
    .WithReference(embeddings)
    .WithEnvironment("Keycloak__AdminUrl", $"{keycloakBaseUrl}/admin/realms/mirai/")
    .WithEnvironment("Keycloak__TokenUrl", $"{keycloakBaseUrl}/realms/mirai/protocol/openid-connect/token")
    .WithEnvironment("Authentication__ValidIssuer", $"{keycloakBaseUrl}/realms/mirai")
    .WithEnvironment("Authentication__MetadataAddress", $"{keycloakBaseUrl}/realms/mirai/.well-known/openid-configuration")
    .WithExternalHttpEndpoints();

builder.AddNpmApp("mirai-app", "../../../mirai-app")
    .WithReference(miraiApi)
    .WaitFor(miraiApi)
    .WithHttpEndpoint(env: "PORT", port: 5173)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();