Write-Host "Setting up user secrets for Mirai..."

Set-Location "src/Presentation"

# Initialize user secrets if not already initialized
dotnet user-secrets init

# Azure Blob Storage
dotnet user-secrets set "Azure:BlobStorage:ContainerName" "<replace-with-container-name>"
dotnet user-secrets set "Azure:BlobStorage:ConnectionString" "<replace-with-connection-string>"

# Keycloak
dotnet user-secrets set "Keycloak:AuthClientSecret" "<replace-with-auth-client-secret>"
dotnet user-secrets set "Keycloak:AdminClientSecret" "<replace-with-admin-client-secret>"

Write-Host "User secrets for Mirai have been set up."