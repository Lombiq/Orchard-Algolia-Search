﻿using Algolia.Search;
using Lombiq.Hosting.AlgoliaSearch.Models;
using Newtonsoft.Json.Linq;
using Orchard.Indexing;
using Orchard.Localization;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment;

namespace Lombiq.Hosting.AlgoliaSearch.Services
{
    public class AlgoliaIndexProvider : IIndexProvider
    {
        private readonly Work<IAlgoliaManager> _algoliaManagerWork;

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }


        public AlgoliaIndexProvider(Work<IAlgoliaManager> algoliaManagerWork)
        {
            _algoliaManagerWork = algoliaManagerWork;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }


        public void CreateIndex(string name)
        {
            var index = _algoliaManagerWork.Value.GetIndex(name);
            var result = index.SetSettingsAsync(new JObject()).Result;
            index.WaitTask(result["taskID"].ToString());
        }

        public ISearchBuilder CreateSearchBuilder(string indexName)
        {
            return new AlgoliaSearchBuilder(indexName, _algoliaManagerWork);
        }

        public void Delete(string indexName, IEnumerable<int> documentIds)
        {
            if (!documentIds.Any())
            {
                return;
            }

            _algoliaManagerWork.Value.GetIndex(indexName).DeleteObjects(documentIds.Select(id => id.ToString()));
        }

        public void Delete(string indexName, int documentId)
        {
            Delete(indexName, new[] { documentId });
        }

        public void DeleteIndex(string name)
        {
            var index = _algoliaManagerWork.Value.GetIndex(name);
            var result = _algoliaManagerWork.Value.DeleteIndex(name);
            index.WaitTask(result["taskID"].ToString());
        }

        public bool Exists(string name)
        {
            return List().Any(index => index == name);
        }

        // Indexed fields can be configured in the Algolia dashboard, so this returns an empty list to avoid confusion.
        public IEnumerable<string> GetFields(string indexName)
        {
            return Enumerable.Empty<string>();
        }

        public bool IsEmpty(string indexName)
        {
            int hitsCount;
            if (int.TryParse((string)_algoliaManagerWork.Value.GetIndex(indexName).Search(new Query(""))["nbHits"], out hitsCount))
            {
                return hitsCount == 0;
            }

            return false;
        }

        public IEnumerable<string> List()
        {
            if (!string.IsNullOrEmpty(_algoliaManagerWork.Value.GetApplicationId()))
            {
                return _algoliaManagerWork.Value.ListIndexes()["items"].Children().Select(c => (string)c["name"]); 
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        public IDocumentIndex New(int documentId)
        {
            return new AlgoliaDocumentIndex(documentId, T);
        }

        public int NumDocs(string indexName)
        {
            return _algoliaManagerWork.Value.GetIndex(indexName).BrowseAll(new Query("")).Count();
        }

        public void Store(string indexName, IDocumentIndex indexDocument)
        {
            Store(indexName, new[] { (AlgoliaDocumentIndex)indexDocument });
        }

        public void Store(string indexName, IEnumerable<IDocumentIndex> indexDocuments)
        {
            Store(indexName, indexDocuments.Cast<AlgoliaDocumentIndex>());
        }

        public void Store(string indexName, IEnumerable<AlgoliaDocumentIndex> indexDocuments)
        {
            indexDocuments = indexDocuments.ToArray();

            if (!indexDocuments.Any())
            {
                return;
            }

            var createdDocuments = new List<JObject>();
            foreach (var indexDocument in indexDocuments)
            {
                createdDocuments.Add(CreateDocument(indexDocument));
            }

            _algoliaManagerWork.Value.GetIndex(indexName).AddObjects(createdDocuments);
            Logger.Debug("Documents [{0}] indexed", string.Join(", ", indexDocuments.Select(document => document.ContentItemId)));
        }


        private JObject CreateDocument(AlgoliaDocumentIndex indexDocument)
        {
            indexDocument.PrepareForIndexing();

            return indexDocument.Document;
        }
    }
}