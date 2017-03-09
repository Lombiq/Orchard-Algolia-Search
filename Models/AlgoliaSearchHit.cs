using Lombiq.Hosting.AlgoliaSearch.Helpers;
using Newtonsoft.Json.Linq;
using Orchard.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lombiq.Hosting.AlgoliaSearch.Models
{
    public class AlgoliaSearchHit : ISearchHit
    {
        private readonly JToken _document;

        public float Score { get; }
        public int ContentItemId { get { return GetInt("objectID"); } }


        public AlgoliaSearchHit(JToken document)
        {
            _document = document;
        }


        public int GetInt(string name)
        {
            var field = _document[name];
            return field == null ? 0 : (int)field;
        }

        public double GetDouble(string name)
        {
            var field = _document[name];
            return field == null ? 0 : (double)field;
        }

        public bool GetBoolean(string name)
        {
            return GetInt(name) > 0;
        }

        public string GetString(string name)
        {
            var field = _document[name];
            return field == null ? null : (string)field;
        }

        public DateTime GetDateTime(string name)
        {
            var field = _document[name];
            return field == null
                ? DateTime.MinValue
                : DateTimeHelper.UnixTimestampToDateTime((double)field);
        }
    }

}