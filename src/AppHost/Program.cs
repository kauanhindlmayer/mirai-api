var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage");

if (builder.ExecutionContext.IsRunMode)
{
    storage.RunAsEmulator(azurite => azurite.WithDataVolume());
}

if (builder.ExecutionContext.IsPublishMode)
{
    var existingStorageName = builder.AddParameter("existingStorageName");
    var existingStorageResourceGroup = builder.AddParameter("existingStorageResourceGroup");
    storage.AsExisting(existingStorageName, existingStorageResourceGroup);
}

storage.AddBlobContainer("files");

var blobs = storage.AddBlobs("blobs");

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

var redis = builder.AddRedis("redis")
    .WithRedisInsight(redisInsight =>
    {
        redisInsight.WithHostPort(6060);
        redisInsight.WithExplicitStart();
    });

var keycloakPassword = builder.AddParameter("KeycloakPassword", secret: true);

var keycloak = builder.AddKeycloak("keycloak", port: 8080, adminPassword: keycloakPassword)
    .WithImageTag("26.2");

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

var ollama = builder.AddOllama("ollama")
    .WithDataVolume()
    .WithContainerRuntimeArgs("--gpus=all");

var llama = ollama.AddModel("chat", "llama3.2:1b");
var minilm = ollama.AddModel("embeddings", "all-minilm");

var keycloakAuthClientSecret = builder.AddParameter("KeycloakAuthClientSecret", secret: true);
var keycloakAdminClientSecret = builder.AddParameter("KeycloakAdminClientSecret", secret: true);

var gitHubClientId = builder.AddParameter("GitHubClientId");
var gitHubClientSecret = builder.AddParameter("GitHubClientSecret", secret: true);

var miraiApi = builder.AddProject<Projects.Presentation>("mirai-api")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithReference(llama)
    .WithReference(minilm)
    .WithReference(blobs)
    .WaitFor(blobs)
    .WithEnvironment("Keycloak__AuthClientSecret", keycloakAuthClientSecret)
    .WithEnvironment("Keycloak__AdminClientSecret", keycloakAdminClientSecret)
    .WithEnvironment("Keycloak__AdminUrl", $"{keycloakBaseUrl}/admin/realms/mirai/")
    .WithEnvironment("Keycloak__TokenUrl", $"{keycloakBaseUrl}/realms/mirai/protocol/openid-connect/token")
    .WithEnvironment("Authentication__ValidIssuer", $"{keycloakBaseUrl}/realms/mirai")
    .WithEnvironment("Authentication__MetadataAddress", $"{keycloakBaseUrl}/realms/mirai/.well-known/openid-configuration")
    .WithEnvironment("GitHub__ClientId", gitHubClientId)
    .WithEnvironment("GitHub__ClientSecret", gitHubClientSecret)
    .WithExternalHttpEndpoints();

var miraiApiUrl = miraiApi.GetEndpoint("https");

var miraiApp = builder.AddNpmApp("mirai-app", "../../../mirai-react-app")
    .WithReference(miraiApi)
    .WaitFor(miraiApi)
    .WithHttpEndpoint(env: "PORT", port: builder.ExecutionContext.IsPublishMode ? 80 : 5173)
    .WithEnvironment("VITE_API_URL", miraiApiUrl)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

if (builder.ExecutionContext.IsPublishMode)
{
    builder.AddAzureContainerAppEnvironment("mirai-env");

    var customDomain = builder.AddParameter("customDomain");
    var certificateName = builder.AddParameter(
        "certificateName",
        value: string.Empty,
        publishValueAsDefault: true);

    miraiApp.PublishAsAzureContainerApp((container, configure) =>
    {
        configure.ConfigureCustomDomain(customDomain, certificateName);
    });

    var insights = builder.AddAzureApplicationInsights("mirai-insights");
    miraiApi.WithReference(insights);

    var miraiAppUrl = miraiApp.GetEndpoint("http");
    miraiApi.WithEnvironment("Cors__AllowedOrigins__0", miraiAppUrl);
}

builder.Build().Run();