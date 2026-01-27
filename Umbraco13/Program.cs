using Microsoft.EntityFrameworkCore;
using Umbraco.Cms.Web.Common.Routing;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure Umbraco request options to handle /funds routes as server-side requests
builder.Services.Configure<UmbracoRequestOptions>(options =>
{
    options.HandleAsServerSideRequest = httpRequest => httpRequest.Path.StartsWithSegments("/funds");
});

builder.Services.AddDbContext<Umbraco13.Data.AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<Umbraco13.Services.IFundService, Umbraco13.Services.FundService>();

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .Build();

WebApplication app = builder.Build();

await app.BootUmbracoAsync();


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
