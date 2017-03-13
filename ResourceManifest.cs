using Orchard.UI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lombiq.Hosting.AlgoliaSearch
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest
                .DefineScript("Algolia.InstantSearch")
                .SetUrl("instantsearch-preact.js", "instantsearch-preact.min.js")
                .SetDependencies("jQuery");

            manifest.DefineStyle("Algolia.InstantSearch").SetUrl("instantsearch.css");
        }
    }
}