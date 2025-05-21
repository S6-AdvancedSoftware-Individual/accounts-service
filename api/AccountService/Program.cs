using HealthChecks.UI.Client;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using PostService.Application.Common.Interfaces;
using Serilog;

var dbUser = Environment.GetEnvironmentVariable("DB_USERID") ?? "";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";
var dbServer = Environment.GetEnvironmentVariable("DB_SERVER") ?? "";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "";
var dbDatabase = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "";
var betterstack_sourceToken = Environment.GetEnvironmentVariable("BETTERSTACK_SOURCETOKEN") ?? "";
var betterstack_endpoint = Environment.GetEnvironmentVariable("BETTERSTACK_ENDPOINT") ?? "";

var builder = WebApplication.CreateBuilder(args);

//Configure SeriLog as the global logger.
Log.Logger = new LoggerConfiguration()
    .WriteTo.BetterStack(
        sourceToken: betterstack_sourceToken,
        betterStackEndpoint: betterstack_endpoint
    )
    .MinimumLevel.Information()
    .CreateLogger();

Log.Information("Starting up the application...");

builder.Logging.ClearProviders();
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add DbContext
var connectionString = $"User Id={dbUser};Password={dbPassword};Server={dbServer};Port={dbPort};Database={dbDatabase}";

builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AccountDbContext>(options =>
    options.UseNpgsql(connectionString));

//Add MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Application.Features.Accounts.CreateAccount).Assembly)
);

// Register DbContext interface
builder.Services.AddScoped<IAccountDbContext>(provider =>
    provider.GetRequiredService<AccountDbContext>());

var app = builder.Build();

// Configure health checks
app.MapHealthChecks("/_health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS middleware
app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
