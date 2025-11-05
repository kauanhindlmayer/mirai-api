using Application;
using Infrastructure;
using Presentation;
using Presentation.Extensions;
using Scalar.AspNetCore;
using Serilog;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddOpenAIServices();

builder.Services.AddSerilog(config =>
    config.ReadFrom.Configuration(builder.Configuration));

builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UsePresentation();
app.UseInfrastructure();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapScalarApiReference(options =>
        options.WithOpenApiRoutePattern("/swagger/1.0/swagger.json"));
}

await app.ApplyMigrationsAsync();
await app.SeedDataAsync();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapDefaultEndpoints();

await app.RunAsync();

public partial class Program;