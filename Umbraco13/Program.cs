using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Umbraco.Cms.Web.Common.Routing;
using Umbraco13.Middleware;
using PdfSharp.Fonts;
using Umbraco13.Fonts;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure Umbraco request options to handle /funds routes as server-side requests
builder.Services.Configure<UmbracoRequestOptions>(options =>
{
    options.HandleAsServerSideRequest = httpRequest => httpRequest.Path.StartsWithSegments("/funds");
});

builder.Services.AddDbContext<Umbraco13.Data.AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<Umbraco13.Services.IFundService, Umbraco13.Services.FundService>();
builder.Services.AddScoped<Umbraco13.Services.IDownloadTokenService, Umbraco13.Services.DownloadTokenService>();

// Add JWT Bearer authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Umbraco13",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "Umbraco13Api",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "your-default-secret-key-change-in-production"))
        };
    });

builder.Services.AddAuthorization();

// Register PdfSharp font resolver
GlobalFontSettings.FontResolver = new FontResolver();

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .Build();

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

// Use API Key authentication middleware before Umbraco middleware
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        // Map controllers with attribute routing (like FundsController with [Route])
        u.EndpointRouteBuilder.MapControllers();

        u.UseInstallerEndpoints();
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
