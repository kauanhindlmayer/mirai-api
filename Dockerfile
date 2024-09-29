FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/WebApi/WebApi.csproj", "WebApi/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Contracts/Contracts.csproj", "Contracts/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]
RUN dotnet restore "WebApi/WebApi.csproj"
COPY . ../
WORKDIR /src/WebApi
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]