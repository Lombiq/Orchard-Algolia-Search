using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin.Notification;
using Orchard.UI.Notify;

namespace Lombiq.Hosting.AlgoliaSearch.Services
{
    public class SettingsNotSetUpBanner : INotificationProvider
    {
        private readonly ISiteService _siteService;
        private readonly UrlHelper _urlHelper;

        public Localizer T { get; set; }


        public SettingsNotSetUpBanner(ISiteService siteService, UrlHelper urlHelper)
        {
            _siteService = siteService;
            _urlHelper = urlHelper;

            T = NullLocalizer.Instance;
        }


        public IEnumerable<NotifyEntry> GetNotifications()
        {
            if (string.IsNullOrEmpty(_siteService.GetAlgoliaSearchSiteSettings().ApplicationId))
            {
                var url = _urlHelper.Action("Index", "Admin", new { Area = "Settings", GroupInfoId = "AlgoliaSearchSettings" });
                yield return new NotifyEntry
                {
                    Message = T("You need to configure <a href=\"{0}\">Algolia Search settings, otherwise search won't work.</a>", url),
                    Type = NotifyType.Warning
                };
            }
        }
    }
}