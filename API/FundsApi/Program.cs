using ClosedXML.Excel;
using FundsApi.Authorization;
using FundsApi.Data;
using FundsApi.Fonts;
using FundsApi.Middleware;
using FundsApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PdfSharp.Fonts;
using Serilog;

// Configure PdfSharp font resolver
GlobalFontSettings.FontResolver = new FontResolver();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting FundsApi application");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add DbContext
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Register Services
    builder.Services.AddScoped<IFundService, FundService>();
    builder.Services.AddScoped<IFundHistoricalNavService, FundHistoricalNavService>();
    builder.Services.AddScoped<IDownloadTokenService, DownloadTokenService>();
    builder.Services.AddScoped<IPdfExportService, PdfExportService>();
    builder.Services.AddScoped<IExcelExportService, ExcelExportService>();

    // Add Controllers
    builder.Services.AddControllers();

    // Configure JWT Authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "fallback-secret-key"))
            };
        });

    builder.Services.AddAuthorization();

    // Configure CORS
    var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(';') ?? new[] { "http://localhost:5000" };
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowUmbracoFrontend", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    // Configure Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Funds API",
            Version = "v1",
            Description = "ASP.NET Core Web API for managing fund data and exports"
        });

        // Add API Key to Swagger
        c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            Description = "API Key authentication using X-API-Key header or api_key query parameter",
            Name = "X-API-Key",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "ApiKeyScheme"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

    // Configure middleware pipeline (CRITICAL ORDER)
    // 1. Exception Handler
    app.UseExceptionHandler("/error");

    // 2. UseCors (MUST be before UseAuthentication)
    app.UseCors("AllowUmbracoFrontend");

    // 3. UseAuthentication
    app.UseAuthentication();

    // 4. UseAuthorization
    app.UseAuthorization();

    // 5. Custom API Key Middleware
    app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

    // 6. Map Controllers
    app.MapControllers();

    // Configure Swagger for Development
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Funds API v1");
            c.RoutePrefix = "swagger";
        });
    }

    // Health check endpoint
    app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
       .WithName("HealthCheck")
       .AllowAnonymous();

    // Error endpoint
    app.MapGet("/error", () => Results.Problem("An error occurred.", statusCode: 500))
       .ExcludeFromDescription();

    Log.Information("FundsApi application configured successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
