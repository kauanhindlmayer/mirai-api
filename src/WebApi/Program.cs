using Application;
using Infrastructure;
using Serilog;
using WebApi;

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
    app.UseInfrastructure();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}