using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace Lombiq.Hosting.AlgoliaSearch.Models
{
    public class AlgoliaSearchSettingsPart : ContentPart
    {
        public string SearchOnlyApiKey
        {
            get { return this.Retrieve(x => x.SearchOnlyApiKey); }
            set { this.Store(x => x.SearchOnlyApiKey, value); }
        }

        public string ApplicationId
        {
            get { return this.Retrieve(x => x.ApplicationId); }
            set { this.Store(x => x.ApplicationId, value); }
        }

        public string EncodedAdminApiKey
        {
            get { return this.Retrieve(x => x.EncodedAdminApiKey); }
            set { this.Store(x => x.EncodedAdminApiKey, value); }
        }

        private readonly LazyField<string> _adminApiKey = new LazyField<string>();
        internal LazyField<string> AdminApiKeyField { get { return _adminApiKey; } }
        public string AdminApiKey { get { return _adminApiKey.Value; } set { _adminApiKey.Value = value; } }
    }
}