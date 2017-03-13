using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lombiq.Hosting.AlgoliaSearch.Models;
using Orchard.ContentManagement;
using Orchard.Settings;

namespace Orchard.Settings
{
    internal static class SiteServiceExtensions
    {
        public static AlgoliaSearchSettingsPart GetAlgoliaSearchSiteSettings(this ISiteService siteService)
        {
            return siteService.GetSiteSettings().As<AlgoliaSearchSettingsPart>();
        }
    }
}