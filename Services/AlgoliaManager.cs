using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Algolia.Search;
using Newtonsoft.Json.Linq;
using Orchard.Settings;
using Lombiq.Hosting.AlgoliaSearch.Models;
using Orchard.ContentManagement;

namespace Lombiq.Hosting.AlgoliaSearch.Services
{
    public class AlgoliaManager : IAlgoliaManager
    {
        private readonly ISiteService _siteService;


        public AlgoliaManager(ISiteService siteService)
        {
            _siteService = siteService;
        }


        public AlgoliaClient GetClient()
        {
            return new AlgoliaClient(GetApplicationId(), GetAdminApiKey());
        }

        public Index GetIndex(string indexName)
        {
            return GetClient().InitIndex(indexName);
        }

        public JObject DeleteIndex(string indexName)
        {
            return GetClient().DeleteIndex(indexName);
        }

        public JObject ListIndexes()
        {
            return GetClient().ListIndexes();
        }

        public string GetSearchOnlyApiKey()
        {
            return GetAlgoliaSearchSiteSettings().SearchOnlyApiKey;
        }

        public string GetApplicationId()
        {
            return GetAlgoliaSearchSiteSettings().ApplicationId;
        }


        private string GetAdminApiKey()
        {
            return GetAlgoliaSearchSiteSettings().AdminApiKeyField.Value;
        }

        private AlgoliaSearchSettingsPart GetAlgoliaSearchSiteSettings()
        {
            return _siteService.GetSiteSettings().As<AlgoliaSearchSettingsPart>();
        }
    }
}