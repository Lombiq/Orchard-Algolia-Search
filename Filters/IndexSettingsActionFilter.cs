using Orchard.Localization;
using Orchard.Mvc.Filters;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lombiq.Hosting.AlgoliaSearch.Filters
{
    public class IndexSettingsActionFilter : FilterProvider, IActionFilter
    {
        private readonly INotifier _notifier;

        public Localizer T { get; set; }


        public IndexSettingsActionFilter(INotifier notifier)
        {
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }


        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var routeValues = filterContext.HttpContext.Request.RequestContext.RouteData.Values;
            if ((string)routeValues["area"] != "Orchard.Indexing" ||
                (string)routeValues["controller"] != "admin" ||
                (string)routeValues["action"] != "index") return;

            _notifier.Warning(T("Use rebuild carefully! After rebuilding an index all your settings will be lost for that particular index so you need to configure it again on the Algolia dashboard."));
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }
    }
}