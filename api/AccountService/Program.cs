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
var dbUser = Environment.GetEnvironmentVariable("DB_USERID") ?? "";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";
var dbServer = Environment.GetEnvironmentVariable("DB_SERVER") ?? "";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "";
var dbDatabase = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "";

// Add DbContext
var connectionString = $"User Id={dbUser};Password={dbPassword};Server={dbServer};Port={dbPort};Database={dbDatabase}";

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
