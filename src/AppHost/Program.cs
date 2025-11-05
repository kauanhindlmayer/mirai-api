var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("mirai-env");

var postgres = builder.AddAzurePostgresFlexibleServer("postgres")
    .RunAsContainer(configure =>
    {
        configure.WithImage("pgvector/pgvector");
        configure.WithImageTag("0.8.0-pg17");
        configure.WithDataVolume();
        configure.WithPgAdmin(pgAdmin =>
        {
            pgAdmin.WithHostPort(5050);
            pgAdmin.WithExplicitStart();
        });
    });

var database = postgres.AddDatabase("mirai-db");

var redis = builder.AddAzureRedis("redis")
    .WithAccessKeyAuthentication()
    .RunAsContainer(configure =>
    {
        configure.WithRedisInsight(redisInsight =>
        {
            redisInsight.WithHostPort(6060);
            redisInsight.WithExplicitStart();
        });
    });

var keycloakPassword = builder.AddParameter("KeycloakPassword", secret: true);

var keycloak = builder.AddKeycloak("keycloak", adminPassword: keycloakPassword)
    .WithImageTag("26.2")
    .WithLifetime(ContainerLifetime.Persistent);

if (builder.ExecutionContext.IsRunMode)
{
    keycloak
        .WithDataVolume()
        .WithRealmImport("../../.files/mirai-realm-export.json");
}

if (builder.ExecutionContext.IsPublishMode)
{
    var postgresUser = builder.AddParameter("PostgresUser", value: "postgres");
    var postgresPassword = builder.AddParameter("PostgresPassword", secret: true);
    postgres.WithPasswordAuthentication(postgresUser, postgresPassword);

    var keycloakDb = postgres.AddDatabase("keycloak-db");

    var keycloakDbUrl = ReferenceExpression.Create(
        $"jdbc:postgresql://{postgres.Resource.HostName}/{keycloakDb.Resource.DatabaseName}");

    keycloak.WithEnvironment("KC_HTTP_ENABLED", "true")
        .WithEnvironment("KC_PROXY_HEADERS", "xforwarded")
        .WithEnvironment("KC_HOSTNAME_STRICT", "false")
        .WithEnvironment("KC_DB", "postgres")
        .WithEnvironment("KC_DB_URL", keycloakDbUrl)
        .WithEnvironment("KC_DB_USERNAME", postgresUser)
        .WithEnvironment("KC_DB_PASSWORD", postgresPassword)
        .WithEndpoint("http", e =>
        {
            e.IsExternal = true;
            e.UriScheme = "https";
        });
}

var keycloakBaseUrl = keycloak.GetEndpoint("http").Property(EndpointProperty.Url);

var openai = builder.AddOpenAI("openai");

var chat = openai.AddModel("chat", "gpt-4o-mini");
var embeddings = openai.AddModel("embeddings", "text-embedding-3-small");

var insights = builder.AddAzureApplicationInsights("mirai-insights");

var blobStorageConnectionString = builder.AddParameter("BlobStorageConnectionString", secret: true);
var blobStorageContainerName = builder.AddParameter("BlobStorageContainerName", value: "files");
var keycloakAuthClientSecret = builder.AddParameter("KeycloakAuthClientSecret", secret: true);
var keycloakAdminClientSecret = builder.AddParameter("KeycloakAdminClientSecret", secret: true);

var miraiApi = builder.AddProject<Projects.Presentation>("mirai-api")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithReference(chat)
    .WithReference(embeddings)
    .WithReference(insights)
    .WithEnvironment("Azure__BlobStorage__ConnectionString", blobStorageConnectionString)
    .WithEnvironment("Azure__BlobStorage__ContainerName", blobStorageContainerName)
    .WithEnvironment("Keycloak__AuthClientSecret", keycloakAuthClientSecret)
    .WithEnvironment("Keycloak__AdminClientSecret", keycloakAdminClientSecret)
    .WithEnvironment("Keycloak__AdminUrl", $"{keycloakBaseUrl}/admin/realms/mirai/")
    .WithEnvironment("Keycloak__TokenUrl", $"{keycloakBaseUrl}/realms/mirai/protocol/openid-connect/token")
    .WithEnvironment("Authentication__ValidIssuer", $"{keycloakBaseUrl}/realms/mirai")
    .WithEnvironment("Authentication__MetadataAddress", $"{keycloakBaseUrl}/realms/mirai/.well-known/openid-configuration")
    .WithExternalHttpEndpoints();

var miraiApiUrl = miraiApi.GetEndpoint("https");

var miraiApp = builder.AddNpmApp("mirai-app", "../../../mirai-app")
    .WithReference(miraiApi)
    .WaitFor(miraiApi)
    .WithHttpEndpoint(env: "PORT", port: 80)
    .WithEnvironment("MIRAI_API_URL", miraiApiUrl)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

if (builder.ExecutionContext.IsPublishMode)
{
    var miraiAppUrl = miraiApp.GetEndpoint("http");
    miraiApi.WithEnvironment("Cors__AllowedOrigins__0", miraiAppUrl);
}

builder.Build().Run();