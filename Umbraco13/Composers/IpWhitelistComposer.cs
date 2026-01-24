using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco13.Middleware;

namespace Umbraco13.Composers;

/// <summary>
/// Composer to register IP whitelist middleware for backoffice access restriction
/// </summary>
public class IpWhitelistComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.Configure<UmbracoPipelineOptions>(options =>
        {
            options.AddFilter(new UmbracoPipelineFilter(
                "IpWhitelist",
                prePipeline: app => app.UseMiddleware<IpWhitelistMiddleware>()
            ));
        });
    }
}
