using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Umbraco13.Extensions
{
    public static class PublishedContentExtensions
    {
        public static HomePage? GetHomePage(this IPublishedContent publishedContent)
        {
            return publishedContent.AncestorOrSelf<HomePage>();
        }

        public static SiteSettingss? GetSiteSettings(this IPublishedContent publishedContent)
        {
            var home = GetHomePage(publishedContent);
            if (home == null) return null;
            return home.FirstChild<SiteSettingss>();
        }
    }
}