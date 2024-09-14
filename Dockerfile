FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Mirai.Api/Mirai.Api.csproj", "Mirai.Api/"]
COPY ["src/Mirai.Application/Mirai.Application.csproj", "Mirai.Application/"]
COPY ["src/Mirai.Domain/Mirai.Domain.csproj", "Mirai.Domain/"]
COPY ["src/Mirai.Contracts/Mirai.Contracts.csproj", "Mirai.Contracts/"]
COPY ["src/Mirai.Infrastructure/Mirai.Infrastructure.csproj", "Mirai.Infrastructure/"]
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]
RUN dotnet restore "Mirai.Api/Mirai.Api.csproj"
COPY . ../
WORKDIR /src/Mirai.Api
RUN dotnet build "Mirai.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mirai.Api.dll"]