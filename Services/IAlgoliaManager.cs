using Algolia.Search;
using Newtonsoft.Json.Linq;
using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lombiq.Hosting.AlgoliaSearch.Services
{
    public interface IAlgoliaManager : IDependency
    {
        /// <summary>
        /// Initializes the client based on the set Application ID and Admin API Key.
        /// </summary>
        /// <returns>The created Algolia client.</returns>
        AlgoliaClient GetClient();

        /// <summary>
        /// Initializes the index on the client.
        /// </summary>
        /// <param name="indexName">The name of the index to be initialized.</param>
        /// <returns>The Algolia index.</returns>
        Index GetIndex(string indexName);

        /// <summary>
        /// Deletes the index with the given name.
        /// </summary>
        /// <param name="indexName">The name of the index to be deleted.</param>
        /// <returns>The result of the deleting process. With this you can e.g.
        /// wait for the deleting task to be completed.</returns>
        JObject DeleteIndex(string indexName);

        /// <summary>
        /// Lists all indexes from client.
        /// </summary>
        /// <returns>The result of the list call which contains the name of the
        /// indexes.</returns>
        JObject ListIndexes();

        /// <summary>
        /// Get the saved Search-Only API Key.
        /// </summary>
        string GetSearchOnlyApiKey();

        /// <summary>
        /// Get the saved Application ID.
        /// </summary>
        string GetApplicationId();
    }
}