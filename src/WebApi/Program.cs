using Application;
using Infrastructure;
using Scalar.AspNetCore;
using Serilog;
using WebApi;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Host.UseSerilog((_, config) =>
        config.ReadFrom.Configuration(builder.Configuration));

    builder.Services
        .AddPresentation(builder.Configuration)
        .AddApplication()
        .AddInfrastructure(builder.Configuration);
}

var app = builder.Build();
{
    app.UseExceptionHandler();
    app.UsePresentation();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(options => options.RouteTemplate = "/openapi/{documentName}.json");
        app.UseSwaggerUI(options =>
        {
            var descriptions = app.DescribeApiVersions();
            foreach (var description in descriptions)
            {
                var url = $"/openapi/{description.GroupName}.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
        });
        app.MapScalarApiReference();
        await app.ApplyMigrationsAsync();
        await app.SeedDataAsync();
    }

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}

public partial class Program;