using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using PostService.Application.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);

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
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "6543";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "accountdb";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "admin";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "admin123";

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

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
