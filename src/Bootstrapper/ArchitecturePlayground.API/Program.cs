using System.Globalization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);
});

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "ArchitecturePlayground API",
        Version = "v1",
        Description = "E-Commerce Platform demonstrating enterprise architecture patterns"
    });
});

// Configure CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// TODO: Add module registrations
// builder.Services.AddIdentityModule(builder.Configuration);
// builder.Services.AddCatalogModule(builder.Configuration);
// builder.Services.AddOrderingModule(builder.Configuration);
// builder.Services.AddBasketModule(builder.Configuration);
// builder.Services.AddPaymentModule(builder.Configuration);
// builder.Services.AddNotificationModule(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
// Enable Swagger in all environments for demo purposes
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ArchitecturePlayground API v1");
    options.RoutePrefix = "swagger";
});

// CORS must be before other middleware
app.UseCors();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
    .WithTags("Health")
    .WithName("HealthCheck");

// Root endpoint
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

// TODO: Map module endpoints
// app.MapIdentityEndpoints();
// app.MapCatalogEndpoints();
// app.MapOrderingEndpoints();
// app.MapBasketEndpoints();
// app.MapPaymentEndpoints();

app.Run();

// Required for integration tests
public partial class Program;
