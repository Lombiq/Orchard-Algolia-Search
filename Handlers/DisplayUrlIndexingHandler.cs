using Orchard.ContentManagement.Handlers;
using Orchard.Mvc.Html;
using System.Web.Mvc;

namespace Lombiq.Hosting.AlgoliaSearch.Handlers
{
    public class DisplayUrlIndexingHandler : ContentHandler
    {
        private readonly UrlHelper _urlHelper;


        public DisplayUrlIndexingHandler(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }


        protected override void Indexing(IndexContentContext context)
        {
            context.DocumentIndex.Add("displayUrl", _urlHelper.ItemDisplayUrl(context.ContentItem));
        }
    }
}