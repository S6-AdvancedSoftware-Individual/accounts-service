using DotNetEnv;
using HealthChecks.UI.Client;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using PostService.Application.Common.Interfaces;
using Serilog;
using Application.Common.Interfaces;

const string API_VERSION = "v2.5";

Env.Load();

var betterstack_sourceToken = Environment.GetEnvironmentVariable("BETTERSTACK_SOURCETOKEN") ?? "";
var betterstack_endpoint = Environment.GetEnvironmentVariable("BETTERSTACK_ENDPOINT") ?? "";

var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "";
var dbDatabase = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";

var messageQueueConnectionString = Environment.GetEnvironmentVariable("COCKATOO_Q") ?? "";
var messageQueueTopic = Environment.GetEnvironmentVariable("COCKATOO_Q_USERNAME_TOPIC") ?? "";

// Add DbContext
var connectionString = $"Host={dbHost};Database={dbDatabase};Username={dbUser};Password={dbPassword}";

var builder = WebApplication.CreateBuilder(args);

//Configure SeriLog as the global logger.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.BetterStack(
        sourceToken: betterstack_sourceToken,
        betterStackEndpoint: betterstack_endpoint
    )
    .MinimumLevel.Information()
    .CreateLogger();

Log.Information("API Version: {Version}", API_VERSION);
Log.Information("Starting up the application...");
Log.Information("Checking environment variables...");

if (string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword) || string.IsNullOrEmpty(dbHost) ||
   string.IsNullOrEmpty(dbDatabase))
{
    Log.Error("Database connection information is missing.");
    throw new SystemException("Application misconfigured, primary database environment variables are missing.");
}

if (string.IsNullOrEmpty(betterstack_endpoint) || string.IsNullOrEmpty(betterstack_sourceToken))
{
    Log.Error("BetterStack connection information is missing.");
    throw new SystemException("Application misconfigured, monitoring & logging environment variables are missing.");
}

if (string.IsNullOrEmpty(messageQueueConnectionString) || string.IsNullOrEmpty(messageQueueTopic))
{
    Log.Error("Azure message bus connection information is missing.");
    throw new SystemException("Application misconfigured, message queue environment variables are missing.");
}

Log.Information("Environment variables checked, all variables are valid.");

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

builder.Services.AddSingleton<IMessagePublisher>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<ServiceBusMessagePublisher>>();
    return new ServiceBusMessagePublisher(
        messageQueueConnectionString,
        messageQueueTopic,
        logger
    );
});

var app = builder.Build();

// Configure health checks
app.MapHealthChecks("/accounts/_health", new HealthCheckOptions
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
