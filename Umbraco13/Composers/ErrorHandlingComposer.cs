using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace Umbraco13.Composers;

public class ErrorHandlingComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.Configure<UmbracoPipelineOptions>(options =>
        {
            options.AddFilter(new UmbracoPipelineFilter(
                "ErrorHandling",
                applicationBuilder =>
                {
                    applicationBuilder.UseStatusCodePagesWithReExecute("/Error/Index?statusCode={0}");
                    applicationBuilder.UseExceptionHandler("/error");
                },
                applicationBuilder =>
                {
                    applicationBuilder.UseExceptionHandler("/Error/Index?statusCode=500");
                }
            ));
        });
    }
}